namespace CourseProjectServer.Model
{
    public class Result
    {
        public TestAttempt Attempt { get; set; } = new();
        public double Mark { get; set; }
        public List<CorrectAnswer> Answers { get; set; } = new();
    }
}
