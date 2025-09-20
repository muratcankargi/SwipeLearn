using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using System.Text.Json;
using System.Text;
using System.Diagnostics;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;
using SwipeLearn.Models.ViewModels;

namespace SwipeLearn.Services
{
    public class MainService
    {
        private readonly HttpClient _httpClient;
        private readonly ITopic _topicRepository;
        private readonly ITopicMaterial _topicMaterialRepository;
        private readonly IServiceProvider _serviceProvider;


        public MainService(IHttpClientFactory httpClientFactory, ITopic repository, ITopicMaterial topicMaterial, IServiceProvider serviceProvider)
        {
            _httpClient = httpClientFactory.CreateClient();
            _topicRepository = repository;
            _topicMaterialRepository = topicMaterial;
            _serviceProvider = serviceProvider;
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
                    using var scope = _serviceProvider.CreateScope();
                    var scopedService = scope.ServiceProvider.GetRequiredService<MainService>();

                    // GetText çağrısı
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
        public async Task<(Guid id, string text, List<string> urls)> Create(Topic topic)
        {
            if (topic == null || topic.Description == null) return (Guid.Empty, "", new List<string>());

            var topicExist = await _topicRepository.GetByDescription(topic.Description);
            if (topicExist != null) return (Guid.Empty, "", new List<string>());

            topic.Id = Guid.NewGuid();
            //await _repository.AddAsync(topic);
            topic.Description = "İstanbul'un Fethi";
            //var text = await GetText(topic.Description);
            var text = "İstanbul'un Fethi, 29 Mayıs 1453 tarihinde gerçekleşmiş, tarihin akışını değiştiren önemli bir olaydır. Osmanlı Padişahı II. Mehmet, şehri kuşatma planları yaparken, tüm dünya bu büyük mücadeleyi merakla izliyordu. Kuşatma sırasında kullanılan devasa toplar, surları aşarak İstanbul'un savunmasını zayıflattı.\n\nFethin en çarpıcı anlarından biri, şehrin surlarının önünde yer alan etkileyici Topkapı Sarayı’dır. Sarayın yüksek ve sağlam surları, bir zamanlar Bizans'ın gücünü simgeliyordu. II. Mehmet’in stratejik liderliği sayesinde, Osmanlı ordusu bu güçlü savunmayı aşmayı başardı. \n\nŞehrin fethi, sadece askeri bir zafer değil, kültürel ve ekonomik bir dönüşümün başlangıcını müjdeliyordu. İstanbul, tüm dünyanın gözdesi haline gelerek, Doğu ile Batı arasında bir köprü işlevi gördü. \n\nSonuç olarak, İstanbul'un Fethi, sadece askeri güç değil, aynı zamanda zeka ve stratejinin de bir zaferiydi. Bu olay, şehirlerin tarihindeki önemli dönüm noktalarından biri olarak günümüzde bile etkisini sürdürmektedir. İstanbul, fetihle birlikte Osmanlı İmparatorluğu’nun kalbi olmuş, tüm dünyanın ilgisini üzerine çekmiştir.";
            //var image_urls = await GenerateImagesAsync(topic.Description, text);
            //var voice = SynthesizeToFileAsync(text);
            //Console.WriteLine(voice);
            List<string> urls = [Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","img07.jpeg"),
               Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "images", "img08.jpg"),
             Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","img09.jpeg"),
             Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","img10.jpeg"),
            ];
            var audioPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices", "elevenlabs_20250920_114819.mp3");

            var path = await CreateVideoAsync(urls, audioPath);
            //Console.WriteLine("video path =" + path);
            return (topic.Id, text, new List<string>());
        }

        //topic to text with chatgpt
        public async Task<string> GetText(string topic, Guid id)
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
                    new { role = "user", content = "Sana vereceğim konu başlığıyla ilgili olarak, yaklaşık 1 dakikalık seslendirmeye uygun bir konuşma metni hazırla. Metin toplamda 120–150 kelime civarında olsun ve en fazla 5 paragraftan oluşsun. İçerik öğretici bir üslup taşısın, giriş–gelişme–sonuç akışına sahip olsun ve öğrencilerin kolayca anlayabileceği bir yapı barındırsın. Ayrıca metinde görseller için ilham verebilecek detaylı betimlemeler yer alsın. Çıktıda yalnızca düzgün paragraf yapısında, tamamen Türkçe bir metin üret; gereksiz semboller veya yabancı dil ifadeleri kesinlikle olmasın. Konu başlığı:"+topic }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var resultJson = await response.Content.ReadAsStringAsync();

            // JSON içinden mesajı çıkar
            using var doc = JsonDocument.Parse(resultJson);
            var description = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            TopicMaterial topicMaterial = new TopicMaterial();
            topicMaterial.Description = description;
            topicMaterial.TopicId = id;
            await _topicMaterialRepository.AddAsync(topicMaterial);

            _ = Task.Run(async () =>
            {
                try
                {
                    await GenerateTextToSpeech(description);
                    await GenerateImagesAsync(topic, description);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"GenerateTextToSpeech failed: {ex.Message}");
                }
            });

            return description;
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


        public async Task<List<string>> GenerateImagesAsync(string topic, string text)
        {
            var prompt = await GetImagePromptText(text);

            var falAiApiKey = Environment.GetEnvironmentVariable("FALAI_API_KEY");
            if (string.IsNullOrEmpty(falAiApiKey))
                throw new Exception("Fal.ai API key is missing. Set the FALAI_API_KEY environment variable.");

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {falAiApiKey}");

            // 4 resmi aynı anda paralel job olarak isteyelim
            var tasks = Enumerable.Range(0, 4).Select(_ => GenerateSingleImageAsync(prompt)).ToList();
            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        private async Task<string> GenerateSingleImageAsync(string prompt)
        {
            var jsonBody = JsonSerializer.Serialize(new
            {
                prompt,
                //image_size = "320x600",
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
                throw new Exception("Fal.ai response_url not found in API response.");

            // Polling: exponential backoff
            int delay = 500;
            for (int i = 0; i < 20; i++) // max ~1 dk
            {
                var pollResp = await _httpClient.GetAsync(responseUrl);
                var pollJson = await pollResp.Content.ReadAsStringAsync();

                try
                {
                    using var pollDoc = JsonDocument.Parse(pollJson);
                    var pollRoot = pollDoc.RootElement;

                    if (pollRoot.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
                    {
                        return images[0].GetProperty("url").GetString()!;
                    }
                }
                catch
                {
                    // JSON parse hatası olursa bekle ve tekrar dene
                }

                await Task.Delay(delay);
                delay = Math.Min(delay * 2, 5000);
            }

            throw new Exception("Fal.ai image was not ready after waiting.");
        }


        public async Task<string> GenerateTextToSpeech(string text, string outputFilePath = null, string outputFormat = "mp3_44100_128")
        {
            try
            {

                if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("text boş olamaz.", nameof(text));
                string voiceId = "dgeCtiGkvIwzoR09qzjl";
                var elevenLabsAiApiKey = Environment.GetEnvironmentVariable("ELEVENLABS_API_KEY");


                outputFilePath ??= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices", $"elevenlabs_{DateTime.UtcNow:yyyyMMdd_HHmmss}.mp3");

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
                await System.IO.File.WriteAllBytesAsync(outputFilePath, bytes);



                Console.WriteLine($"ElevenLabs TTS tamamlandı. Dosya: {outputFilePath} ");

                return outputFilePath;

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public async Task<string> CreateVideoAsync(List<string> imagePaths, string audioPath, string outputPath = null)
        {
            if (imagePaths == null || imagePaths.Count == 0)
                throw new ArgumentException("En az 1 görsel gerekli", nameof(imagePaths));
            if (string.IsNullOrEmpty(audioPath) || !File.Exists(audioPath))
                throw new ArgumentException("Ses dosyası bulunamadı", nameof(audioPath));

            outputPath ??= Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos",
                $"output_{DateTime.UtcNow:yyyyMMdd_HHmmss}.mp4");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            // FFmpeg dosyalarının hazır olduğundan emin ol
            //await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);

            // 1. Ses uzunluğunu al
            var audioInfo = await FFmpeg.GetMediaInfo(audioPath);
            double audioDuration = audioInfo.Duration.TotalSeconds;

            // 2. Her resim için video klip oluştur (12 saniye örnek)
            var tempVideos = new List<string>();
            double perImageDuration = audioDuration / imagePaths.Count;

            for (int i = 0; i < imagePaths.Count; i++)
            {
                string tempVideo = Path.Combine(Path.GetTempPath(), $"img_{i}.mp4");
                tempVideos.Add(tempVideo);
                Console.WriteLine($"Image {i}: duration={perImageDuration}");
                await FFmpeg.Conversions.New()
                    .AddParameter($"-loop 1 -t {perImageDuration.ToString(System.Globalization.CultureInfo.InvariantCulture)} -i \"{imagePaths[i]}\"")
                    .AddParameter("-c:v libx264 -pix_fmt yuv420p")
                    .SetOutput(tempVideo)
                    .Start();
            }

            // 3. Tüm image videoları birleştir
            string concatFile = Path.Combine(Path.GetTempPath(), "concat.txt");
            await File.WriteAllLinesAsync(concatFile, tempVideos.Select(v => $"file '{v.Replace("\\", "/")}'"));

            string tempConcatVideo = Path.Combine(Path.GetTempPath(), "concat_video.mp4");
            await Task.Delay(200);
            await FFmpeg.Conversions.New()
                .AddParameter($"-f concat -safe 0 -i \"{concatFile}\" -c:v libx264 -pix_fmt yuv420p \"{tempConcatVideo}\"")
                .Start();
            await Task.Delay(200);
            // 4. Ses ekle
            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{tempConcatVideo}\" -i \"{audioPath}\" -c:v copy -c:a aac -shortest \"{outputPath}\"")
                .Start();

            // Geçici dosyaları temizleyebilirsin
            foreach (var temp in tempVideos.Append(tempConcatVideo))
                if (File.Exists(temp)) File.Delete(temp);

            return outputPath;
        }



        public async Task<TopicInfoItem> GetStructuredTopicInfoAsync(Guid id)
        {
            TopicMaterial? topicModel = null;
            for (int i = 0; i < 30; i++) // ~30 sn
            {
                topicModel = await _topicMaterialRepository.GetByTopicId(id);
                if (topicModel != null && !string.IsNullOrEmpty(topicModel.Description))
                    break;

                await Task.Delay(1000); 
            }

            var apiKey = Environment.GetEnvironmentVariable("CHATGPT_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("OpenAI API key is missing.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-2024-08-06",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant. Respond ONLY with a JSON array of short strings. Do not include any markdown or extra text." },
                    new { role = "user", content = $"Provide 3–10 important points about this topic as a JSON array of short strings: {topicModel.Description}" }
                },
                temperature = 0.3
            };


            string? content = null;

            for (int attempt = 0; attempt < 20; attempt++) // polling: (~20 sn)
            {
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

                if (!string.IsNullOrEmpty(content))
                    break;

                await Task.Delay(1000);
            }


            if (string.IsNullOrEmpty(content))
                return new TopicInfoItem();

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
                return new TopicInfoItem();

            return new TopicInfoItem()
            {
                Info = points
            };
        }


    }
}