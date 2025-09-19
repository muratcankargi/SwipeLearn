using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Repositories;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SwipeLearn.Services
{
    public class TopicService
    {
        private readonly ITopic _repository;
        private readonly HttpClient _httpClient;

        public TopicService(ITopic repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<(Guid id, string text, List<string> urls)> Create(Topic topic)
        {
            if (topic == null || topic.Description == null) return (Guid.Empty, "", new List<string>());

            var topicExist = await _repository.GetByDescription(topic.Description);
            if (topicExist != null) return (Guid.Empty, "", new List<string>());

            topic.Id = Guid.NewGuid();
            await _repository.AddAsync(topic);
            var text = await GetText(topic.Description);

            var image_urls = await GenerateImagesAsync(topic.Description, text);
            return (topic.Id, text, image_urls);
        }

        //topic to text with chatgpt
        public async Task<string> GetText(string description)
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
                    new { role = "user", content = "Sana vereceğim konu başlığıyla ilgili olarak, en fazla 5 paragraf olacak şekilde öğretici bir içerik hazırla. Metin, konu başlığını detaylı açıklasın, önemli noktaları vurgulasın ve öğrencilerin kolay anlayabileceği örnekler içersin. Her paragraf mantıklı bir akışa sahip olsun: giriş, gelişme ve sonuç bölümleri olsun. Ayrıca metin, bu konuyla alakalı görsel fikirler için de ilham verebilecek detaylı betimlemeler içersin. Başlık:"+description }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var resultJson = await response.Content.ReadAsStringAsync();

            // JSON içinden mesajı çıkar
            using var doc = JsonDocument.Parse(resultJson);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return message;

        }

        //text to image with fal.ai (flux-pro)
        public async Task<List<string>> GenerateImagesAsync(string topic, string text)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            var falAiApiKey = Environment.GetEnvironmentVariable("FALAI_API_KEY");
            Console.WriteLine("token = " + falAiApiKey);
            if (string.IsNullOrEmpty(falAiApiKey))
                throw new Exception("Fal.ai API key is missing. Set the FALAI_API_KEY environment variable.");

            var prompt = $"Konu: {topic}. Açıklama: {text}. Bu konuya uygun, detaylı ve görsel olarak zengin bir sahneyi hayal et. Renkleri, ışıklandırmayı ve ortamı net şekilde betimle. Odak noktaları belirgin olsun ve görsel olarak etkileyici bir kompozisyon oluştur.";

            var jsonBody = JsonSerializer.Serialize(new
            {
                prompt,
                image_size = new { width = 430, height = 932 },
                num_images = 4
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://queue.fal.run/fal-ai/flux-pro/v1.1-ultra")
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Key {falAiApiKey}");

            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;

            if (root.TryGetProperty("detail", out var detailMsg))
                throw new Exception($"Fal.ai API error: {detailMsg.GetString()}");

            var responseUrl = root.GetProperty("response_url").GetString();
            if (string.IsNullOrEmpty(responseUrl))
                throw new Exception("Fal.ai response_url not found in API response.");

            // polling
            for (int i = 0; i < 30; i++) // (~1 minute)
            {
                var pollResp = await _httpClient.GetAsync(responseUrl);
                var pollJson = await pollResp.Content.ReadAsStringAsync();
                using var pollDoc = JsonDocument.Parse(pollJson);
                var pollRoot = pollDoc.RootElement;

                if (pollRoot.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
                {
                    return images
                        .EnumerateArray()
                        .Select(img => img.GetProperty("url").GetString()!)
                        .ToList();
                }

                await Task.Delay(2000); 
            }

            throw new Exception("Fal.ai images were not ready after waiting.");
        }



    }

}
