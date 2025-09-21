using System.Text.Json.Serialization;

namespace SwipeLearn.Utils
{
    public class QuestionJson
    {
        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("answers")]
        public string[] Answers { get; set; }

        [JsonPropertyName("correct")]
        public string Correct { get; set; }
    }
}
