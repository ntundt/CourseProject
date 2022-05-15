namespace CourseProjectServer.Model
{
    public class Test
    {
        public int TestId { get; set; }
        public User Author { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public bool ResultsPublic { get; set; }
        public bool CanNotReviewQuestion { get; set; }
        public int Attempts { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public DateTime PublicUntil { get; set; } = new DateTime(1900, 1, 1);
        public DateTime PrivateUntil { get; set; } = new DateTime(1900, 1, 1);
        public List<Question> Questions { get; set; } = new();
    }
}
