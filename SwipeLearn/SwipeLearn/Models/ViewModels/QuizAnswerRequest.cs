namespace SwipeLearn.Models.ViewModels
{
    public class QuizAnswerRequest
    {
        public Guid Id { get; set; }
        public int QuestionIndex { get; set; }
        public int OptionIndex { get; set; }
    }

    public class QuizAnswerResponse
    {
        public bool IsCorrect { get; set; }
    }

}
