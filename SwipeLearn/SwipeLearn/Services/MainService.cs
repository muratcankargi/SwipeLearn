using Microsoft.EntityFrameworkCore;
using SwipeLearn.Context;
using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Models.ViewModels;
using SwipeLearn.Repositories;
using SwipeLearn.Utils;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using static SwipeLearn.Utils.Enums;

namespace SwipeLearn.Services
{
    public class MainService
    {
        private readonly HttpClient _httpClient;
        private readonly ITopic _topicRepository;
        private readonly ITopicMaterial _topicMaterialRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IVideo _videoRepository;
        private readonly IQuestion _questionRepository;

        public MainService(IHttpClientFactory httpClientFactory, ITopic repository, ITopicMaterial topicMaterial, IServiceScopeFactory scopeFactory, IVideo videoRepository, IQuestion questionRepository)
        {
            _httpClient = httpClientFactory.CreateClient();
            _topicRepository = repository;
            _topicMaterialRepository = topicMaterial;
            _scopeFactory = scopeFactory;
            _videoRepository = videoRepository;
            _questionRepository = questionRepository;
        }
        public async Task<TopicGuid> CreateTopic(Topic topic)
        {
            if (topic == null || topic.Description == null) return (new TopicGuid());

            //var topicExist = await _repository.GetByDescription(topic.Description);
            //if (topicExist != null) return (Guid.Empty, "", new List<string>());

            topic.Id = Guid.NewGuid();
            await _topicRepository.AddAsync(topic);
            TopicGuid model = new TopicGuid();
            model.Id = topic.Id;
            _ = Task.Run(async () => //fire and forget
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var scopedService = scope.ServiceProvider.GetRequiredService<MainService>();

                    await scopedService.GetText(topic.Description, model.Id);

                }
                catch (Exception ex)
                {
                    // Hataları logla ama main akışı etkileme
                    Console.WriteLine($"GetText failed: {ex.Message}");
                }
            });
            return model;

        }

        //topic to text with chatgpt
        public async Task<List<string>> GetText(string topic, Guid id)
        {
            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
       new { role = "system", content = "You are a helpful teacher." },
       new { role = "user", content =
           "Sana vereceğim konu başlığıyla ilgili 120-150 kelimelik öğretici bir konuşma hazırla. " +
           "Metni 3 parçaya ayır, giriş–gelişme–sonuç akışı olsun. " +
           $"Konu başlığı: {topic}" }
   },
                temperature = 0.4,


                response_format = new
                {
                    type = "json_schema",
                    json_schema = new
                    {
                        name = "text_parts",
                        schema = new
                        {
                            type = "object",
                            properties = new
                            {
                                parts = new
                                {
                                    type = "array",
                                    items = new { type = "string" }
                                }
                            },
                            required = new[] { "parts" },
                            additionalProperties = false
                        }
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var resultJson = await response.Content.ReadAsStringAsync();


            using var doc = JsonDocument.Parse(resultJson);
            var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

            var parsedJson = JsonDocument.Parse(content);
            var parts = parsedJson.RootElement
                .GetProperty("parts")
                .EnumerateArray()
                .Select(x => x.GetString()!)
                .ToList();

            // Kaydet
            TopicMaterial topicMaterial = new TopicMaterial
            {
                TopicId = id,
                Description = parts // veya ayrı ayrı kaydedebilirsin
            };
            await _topicMaterialRepository.AddAsync(topicMaterial);

            _ = Task.Run(() => StartPostProcessingAsync(id, topic, parts));

            return parts;
        }



        private async Task StartPostProcessingAsync(Guid id, string topic, List<string> descriptionParts)
        {
            try
            {
                Console.WriteLine("Question Oluşturma Başladı");
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedService = scope.ServiceProvider.GetRequiredService<MainService>();
                    await scopedService.GenerateAndSaveQuestionsAsync(id, string.Join("\n\n", descriptionParts));
                }
                Console.WriteLine("Question Oluşturma Bitti");

                // Videoları sırayla oluştur
                for (int index = 0; index < descriptionParts.Count; index++)
                {
                    var part = descriptionParts[index];
                    Console.WriteLine($"Video {index + 1} için ses ve resim işlemleri başlıyor");

                    try
                    {
                        using var videoScope = _scopeFactory.CreateScope();
                        var scopedService = videoScope.ServiceProvider.GetRequiredService<MainService>();

                        await scopedService.GenerateTextToSpeech(part, id);
                        await scopedService.GenerateAndSaveImagesAsync(id, topic, part);
                        await scopedService.CreateVideosAsync(id, index);

                        Console.WriteLine($"Video {index + 1} işlemi tamamlandı");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Video {index + 1} işlemi başarısız: {ex.Message}");
                    }
                }

                // ⬇️ For döngüsünden sonra isVideosReady güncelle
                using (var updateScope = _scopeFactory.CreateScope())
                {
                    var db = updateScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var topicEntity = await db.Topics.FindAsync(id);
                    if (topicEntity != null)
                    {
                        topicEntity.IsVideosReady = true;
                        await db.SaveChangesAsync();
                    }
                }
                Console.WriteLine("isVideosReady güncellendi ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PostProcessing failed: {ex.Message}");
            }
        }




        public async Task<string> GetImagePromptText(string description)
        {
            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "You are an expert visual prompt generator for AI image creation." },
            new
            {
                role = "user",
                content =
                $"I will give you a topic. Based on it, create a detailed and inspiring English prompt suitable for generating a single high-quality AI image. " +
                $"The prompt must describe the scene vividly with clear subjects, atmosphere, background, lighting, mood, and artistic style. " +
                $"Avoid technical jargon. Focus on making it visually rich and creative. " +
                $"Topic: {description}"
            }
        }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var resultJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(resultJson);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message;
        }

        public async Task<List<string>> GenerateAndSaveImagesAsync(Guid topicId, string topic, string text)
        {
            var topicMaterial = await _topicMaterialRepository.GetByTopicId(topicId);
            if (topicMaterial == null)
                Console.WriteLine("TopicMaterial not found.");

            var imageUrls = await GenerateImagesAsync(topic, text);

            // 3. Images alanına ekle veya oluştur
            topicMaterial.Images ??= new List<string>();
            topicMaterial.Images.AddRange(imageUrls);

            // 4. Veritabanına kaydet
            await _topicMaterialRepository.UpdateAsync(topicMaterial);

            return imageUrls;
        }

        public async Task<List<string>> GenerateImagesAsync(string topic, string text)
        {
            var prompt = await GetImagePromptText(text);

            var falAiApiKey = Environment.GetEnvironmentVariable("FALAI_API_KEY");
            if (string.IsNullOrEmpty(falAiApiKey))
                throw new Exception("Fal.ai API key is missing.");

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {falAiApiKey}");

            var tasks = Enumerable.Range(0, 4).Select(_ => GenerateAndSaveSingleImageAsync(prompt)).ToList();
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<string> GenerateAndSaveSingleImageAsync(string prompt)
        {
            var jsonBody = JsonSerializer.Serialize(new
            {
                prompt,
                image_size = new { width = 320, height = 600 },
                num_images = 1
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://queue.fal.run/fal-ai/flux-1/schnell")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("detail", out var detailMsg))
                throw new Exception($"Fal.ai API error: {detailMsg.GetString()}");

            var responseUrl = root.GetProperty("response_url").GetString();
            if (string.IsNullOrEmpty(responseUrl))
                throw new Exception("Fal.ai response_url not found.");

            // Polling: exponential backoff
            int delay = 500;
            string imageUrl = null!;
            for (int i = 0; i < 20; i++)
            {
                var pollResp = await _httpClient.GetAsync(responseUrl);
                var pollJson = await pollResp.Content.ReadAsStringAsync();

                try
                {
                    using var pollDoc = JsonDocument.Parse(pollJson);
                    var pollRoot = pollDoc.RootElement;

                    if (pollRoot.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
                    {
                        imageUrl = images[0].GetProperty("url").GetString()!;
                        break;
                    }
                }
                catch { }

                await Task.Delay(delay);
                delay = Math.Min(delay * 2, 5000);
            }

            if (string.IsNullOrEmpty(imageUrl))
                throw new Exception("Image was not ready after waiting.");

            // Resmi indir
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

            // Kaydetmek için dosya adı üret
            var fileName = $"{Guid.NewGuid()}.png";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            // wwwroot/images klasörünün varlığını kontrol et
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            await File.WriteAllBytesAsync(path, imageBytes);

            // Dosya adını return et
            return fileName;
        }

        public async Task<string> GenerateTextToSpeech(string text, Guid id, string outputFilePath = null, string outputFormat = "mp3_44100_128")
         {
            try
            {

                if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("text boş olamaz.", nameof(text));
                string voiceId = "dgeCtiGkvIwzoR09qzjl";
                var elevenLabsAiApiKey = Environment.GetEnvironmentVariable("ELEVENLABS_API_KEY");


                outputFilePath ??= Path.Combine($"elevenlabs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.mp3");

                var endpoint = $"https://api.elevenlabs.io/v1/text-to-speech/{Uri.EscapeDataString(voiceId)}?output_format={Uri.EscapeDataString(outputFormat)}";

                //  voice settings
                var requestBody = new
                {
                    text = text,
                    model_id = "eleven_multilingual_v2",
                    language_code = "tr",
                    use_speaker_boost = true

                };
                var json = JsonSerializer.Serialize(requestBody);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("xi-api-key");
                _httpClient.DefaultRequestHeaders.Add("xi-api-key", elevenLabsAiApiKey);

                using var response = await _httpClient.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"ElevenLabs TTS isteği başarısız. Status: {response.StatusCode}, Body: {err}");
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices", outputFilePath);
                await System.IO.File.WriteAllBytesAsync(savePath, bytes);


                var topicMaterial = await _topicMaterialRepository.GetByTopicId(id);
                topicMaterial.Voice ??= new List<string>();

                topicMaterial.Voice.Add(outputFilePath);

                await _topicMaterialRepository.UpdateAsync(topicMaterial);

                Console.WriteLine($"ElevenLabs TTS tamamlandı. Dosya: {outputFilePath} ");

                return outputFilePath;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public async Task<string> CreateVideosAsync(Guid topicId, int videoIndex)
        {
            var topic = await _topicMaterialRepository.GetByTopicId(topicId);

            if (topic.Images == null || topic.Images.Count == 0)
                throw new ArgumentException("En az 1 görsel gerekli", nameof(topic.Images));
            if (topic.Voice == null || topic.Voice.Count == 0)
                throw new ArgumentException("En az 1 ses gerekli", nameof(topic.Voice));

            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            Directory.CreateDirectory(outputDir);

            // Tek bir ses dosyası seçelim (örn. ilkini)  topic.Voice[videoIndex]
            var audioPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices", topic.Voice[videoIndex]);

            // Ses uzunluğunu öğren
            var audioInfo = await FFmpeg.GetMediaInfo(audioPath);
            double audioDuration = audioInfo.Duration.TotalSeconds;

            // Resim başına süre
            double perImageDuration = audioDuration / 4;

            // Geçici videolar oluştur
            var tempVideos = new List<string>();
            for (int j = videoIndex * 4; j < topic.Images.Count; j++)
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", topic.Images[j]);
                string tempVideo = Path.Combine(Path.GetTempPath(), $"img_{j}.mp4");
                tempVideos.Add(tempVideo);

                await FFmpeg.Conversions.New()
                    .AddParameter($"-loop 1 -t {perImageDuration.ToString(System.Globalization.CultureInfo.InvariantCulture)} -i \"{imagePath}\"")
                    .AddParameter("-c:v libx264 -pix_fmt yuv420p")
                    .SetOutput(tempVideo)
                    .Start();
            }

            // concat dosyası
            string concatFile = Path.Combine(Path.GetTempPath(), "concat.txt");
            await File.WriteAllLinesAsync(concatFile, tempVideos.Select(v => $"file '{v.Replace("\\", "/")}'"));

            string tempConcatVideo = Path.Combine(Path.GetTempPath(), "concat.mp4");
            await FFmpeg.Conversions.New()
                .AddParameter($"-f concat -safe 0 -i \"{concatFile}\" -c:v libx264 -pix_fmt yuv420p \"{tempConcatVideo}\"")
                .Start();

            // Son çıktı (tek video)
            string outputPath = Path.Combine(outputDir, $"video_{DateTime.UtcNow:yyyyMMdd_HHmmss}.mp4");
            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{tempConcatVideo}\" -i \"{audioPath}\" -c:v copy -c:a aac -shortest \"{outputPath}\"")
                .Start();

            // DB'ye kaydet
            var videoEntity = new Video
            {
                TopicId = topicId,
                VideoPath = Path.GetFileName(outputPath)
            };
            await _videoRepository.AddAsync(videoEntity);

            // geçici dosyaları temizle
            foreach (var temp in tempVideos.Append(tempConcatVideo))
                if (File.Exists(temp)) File.Delete(temp);

            return videoEntity.VideoPath;
        }

        public async Task<TopicInfoItem> GetStructuredTopicInfoAsync(Guid id)
        {
            Topic? topicModel = null;
            topicModel = await _topicRepository.GetById(id);
            if (topicModel == null)
                return null;


            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("OpenAI API key is missing.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4.1-nano",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant. Respond ONLY with a JSON array of short strings. Do not include any markdown or extra text." },
                     new { role = "user", content = $"'{topicModel.Description}' hakkında en az 3, en fazla 10 tane kısa ve net bilgi maddesi üret. Çıktıyı yalnızca JSON array formatında ve tamamen Türkçe olarak ver." }
                },
                temperature = 0.3
            };


            string? content = null;

            var response = await httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                content = JsonDocument.Parse(json)
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch
            {
                content = null;
            }

            if (string.IsNullOrEmpty(content))
                return null;

            // Markdown veya code block temizleme
            content = content.Trim();
            if (content.StartsWith("```"))
            {
                int firstLineBreak = content.IndexOf('\n');
                int lastTriple = content.LastIndexOf("```");
                if (firstLineBreak >= 0 && lastTriple > firstLineBreak)
                {
                    content = content.Substring(firstLineBreak + 1, lastTriple - firstLineBreak - 1).Trim();
                }
            }

            List<string>? points = null;
            try
            {
                points = JsonSerializer.Deserialize<List<string>>(content);
            }
            catch
            {
                // fallback: eğer model valid JSON vermediyse, satır satır ayırıp liste yap
                points = content.Split('\n')
                                .Select(x => x.Trim().TrimStart('-', '*', '•', '–').Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToList();
            }

            if (points == null || points.Count == 0)
                return null;

            return new TopicInfoItem()
            {
                Info = points
            };
        }

        public async Task<bool> IsVideosReady(Guid topicId)
        {
            var topic = await _topicRepository.GetById(topicId);

            if (topic == null)
                return false; // böyle bir topic yoksa false döndür

            return topic.IsVideosReady; // direkt kolon değerini dön
        }


        public async Task<VideoUrls?> GetVideoByTopicId(Guid topic_id)
        {
            var videoUrlPaths = await _videoRepository.GetVideoPathsByTopicIdAsync(topic_id);
            if (videoUrlPaths == null || videoUrlPaths.Count == 0)
                return null;

            var fullPaths = videoUrlPaths
                .Select(path => Path.Combine("videos", path))
                .ToList();

            return new VideoUrls
            {
                videoUrls = fullPaths
            };
        }

        public async Task<List<Question>> GenerateAndSaveQuestionsAsync(Guid topicId, string description)
        {
            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("OpenAI API key is missing.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                 {
                    new { role = "system", content = "You are a helpful quiz generator. Respond ONLY with valid JSON, no explanations." },
                    new { role = "user", content =
                        $@"Konu: {description}.
                        Bu konuya uygun olarak 10 adet çoktan seçmeli soru üret. 

                        Kurallar:
                        - Sorular kolaydan zora doğru sıralansın (1. soru en kolay, 10. soru en zor olsun).
                        - Sorular çok kısa olmamalı, açıklayıcı ve öğretici bir üslup taşımalı.
                        - Eğer önemli ek bilgiler veya kritik ayrıntılar varsa, bunları sorulara yansıt.
                        - Her soru için 4 tane şık üret (A, B, C, D).
                        - Doğru cevaplar her zaman aynı şık olmasın, karışık dağıtılsın.
                        - Çıktıyı sadece şu JSON formatında ver, başka açıklama ekleme:

                        [
                            {{
                                ""question"": ""...soru metni..."",
                                ""answers"": [""A şıkkı"", ""B şıkkı"", ""C şıkkı"", ""D şıkkı""],
                                ""correct"": ""A,B,C veya D"" 
                            }},
                            ...
                        ]"
                    }
                },
                temperature = 0.7
            };


            var response = await httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var json = await response.Content.ReadAsStringAsync();

            string? content = null;
            try
            {
                content = JsonDocument.Parse(json)
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
            catch
            {
                throw new Exception("ChatGPT response parse error.");
            }

            if (string.IsNullOrEmpty(content))
                throw new Exception("ChatGPT did not return content.");

            // JSON code block temizleme
            content = content.Trim();
            if (content.StartsWith("```"))
            {
                int firstLineBreak = content.IndexOf('\n');
                int lastTriple = content.LastIndexOf("```");
                if (firstLineBreak >= 0 && lastTriple > firstLineBreak)
                {
                    content = content.Substring(firstLineBreak + 1, lastTriple - firstLineBreak - 1).Trim();
                }
            }

            List<Question>? generatedQuestions;
            try
            {
                var parsed = JsonSerializer.Deserialize<List<QuestionJson>>(content);
                if (parsed == null) throw new Exception("JSON boş geldi.");

                generatedQuestions = parsed.Select(q => new Question
                {
                    TopicId = topicId,
                    QuestionText = q.Question,
                    Answers = q.Answers,
                    Correct = q.Correct
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("ChatGPT output could not be deserialized.", ex);
            }

            foreach (var q in generatedQuestions)
            {
                await _questionRepository.AddAsync(q);
            }

            return generatedQuestions;
        }


        public async Task<QuizResponse> GetQuizByTopicIdAsync(Guid topicId)
        {
            var questions = await _questionRepository.GetQuestionsByTopicIdAsync(topicId);

            if (questions == null || questions.Count == 0)
                return new QuizResponse();

            var response = new QuizResponse
            {
                Questions = questions.Select(q => new QuizItem
                {
                    Question = q.QuestionText ?? string.Empty,
                    Options = q.Answers.ToList()
                }).ToList()
            };

            return response;
        }

        // Böyle olunca kullanıcı yanlış şıkkı işaretleyince doğrusunu gösteremiyoruz

        public async Task<QuizAnswerResponse> CheckAnswerAsync(QuizAnswerRequest request)
        {
            var questions = await _questionRepository.GetQuestionsByTopicIdAsync(request.Id);

            if (questions == null || questions.Count == 0)
                return new QuizAnswerResponse { CorrectOptionIndex = -1 };

            if (request.QuestionIndex < 0 || request.QuestionIndex >= questions.Count)
                return new QuizAnswerResponse { CorrectOptionIndex = -1 };

            var question = questions[request.QuestionIndex];

            if (request.OptionIndex < 0 || request.OptionIndex >= question.Answers.Length)
                return new QuizAnswerResponse { CorrectOptionIndex = -1 };

            OptionLetter letter = Enum.Parse<OptionLetter>(question.Correct);
            int index = (int)letter;

            return new QuizAnswerResponse { CorrectOptionIndex = index };
        }

        public async Task<ListPagination<TopicResponseDto>> GetTopics()
        {
            var topics = await _topicRepository.GetAll();
            return topics;
        }

        public async Task<QuizDescriptionAnswerResponse> GetQuizAnswerDescription(QuizAnswerRequest request)
        {
            // 1. Soruyu DB’den bul
            var question = await _questionRepository.GetByTopicAndIndexAsync(request.Id, request.QuestionIndex);
            if (question == null)
                throw new Exception("Soru bulunamadı");

            // 2. Doğru cevabı al
            OptionLetter letter = Enum.Parse<OptionLetter>(question.Correct);
            int index = (int)letter;
            var correctAnswer = question.Answers[index];
            var userAnswer = question.Answers[request.OptionIndex];

            // 3. ChatGPT prompt hazırla
            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var prompt = $@"
                Soru: {question.QuestionText}
                Kullanıcının seçtiği cevap: {userAnswer}
                Doğru cevap: {correctAnswer}

                Görevin: Doğru cevabın neden doğru olduğunu öğretici bir şekilde 2-3 cümleyle açıkla.
                Yanıtında sadece açıklamayı döndür. Kullanıcının verdiği yanıt yanlışsa neden yanlış olduğunu da açıkla.
                ";

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "You are a helpful teacher." },
            new { role = "user", content = prompt }
        },
                temperature = 0.5
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var resultJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(resultJson);
            var description = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new QuizDescriptionAnswerResponse
            {
                Description = description ?? "Açıklama üretilemedi."
            };
        }


    }
}