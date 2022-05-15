namespace CourseProjectServer.Model
{
    public class UserAnswer
    {
        public int UserAnswerId { get; set; }
        public TestAttempt TestAttempt { get; set; } = new();
        public List<AnswerOption>? AnswerOption { get; set; }
        public Question? Question { get; set; }
        public string? Answer { get; set; }
    }
}
