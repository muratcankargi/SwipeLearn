using SwipeLearn.Interfaces;
using SwipeLearn.Models;
using SwipeLearn.Repositories;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace SwipeLearn.Services
{
    public class TopicService
    {
        private readonly ITopic _repository;

        public TopicService(ITopic repository)
        {
            _repository = repository;
        }

        public async Task<(Guid id, string text)> Create(Topic topic)
        {
            if (topic == null || topic.Description == null) return (Guid.Empty, "");

            var topicExist = await _repository.GetByDescription(topic.Description);
            if (topicExist != null) return (Guid.Empty, "");

            topic.Id = Guid.NewGuid();
            await _repository.AddAsync(topic);
            var text = await GetText(topic.Description) ;
            return (topic.Id, text);
        }

        public async Task<string> GetText(string description)
        {
            var apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful teacher." },
                    new { role = "user", content = "Sana vereceğim konu başlığıyla alakalı en fazla 5 paragraf olacak şekilde konuyu anlat. Başlık:"+description }
                }
            };

            var response = await http.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
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

    }

}
