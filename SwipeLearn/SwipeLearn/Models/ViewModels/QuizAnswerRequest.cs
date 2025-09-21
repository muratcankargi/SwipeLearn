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
        public int CorrectOptionIndex { get; set; }
    }

    public class QuizDescriptionAnswerResponse
    {
        public string Description { get; set; }
    }

}
