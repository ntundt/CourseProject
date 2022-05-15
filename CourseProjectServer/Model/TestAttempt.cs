namespace CourseProjectServer.Model
{
    public class TestAttempt
    {
        public int AttemptId { get; set; }
        public User Testee { get; set; } = new();
        public Test Test { get; set; } = new();
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
    }
}
