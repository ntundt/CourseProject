namespace DataTransferObject
{
    public class TestInfo
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public long CreatedDate { get; set; }
        public bool ResultsPublic { get; set; }
        public bool CanNotReviewQuestions { get; set; }
        public int Attempts { get; set; }
        public long TimeLimit { get; set; }
        public long PrivateUntil { get; set; }
        public long PublicUntil { get; set; }
        public int? QuestionCount { get; set; }
    }
}
