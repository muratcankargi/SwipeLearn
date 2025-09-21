namespace SwipeLearn.Models.ViewModels
{
    public class QuizResponse
    {
        public List<QuizItem> Questions { get; set; } = new();
    }

    public class QuizItem
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }

}
