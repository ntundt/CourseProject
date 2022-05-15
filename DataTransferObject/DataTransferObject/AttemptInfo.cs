namespace DataTransferObject
{
    public class AttemptInfo
    {
        public int AttemptId { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
        public long Started { get; set; }
        public long? Ended { get; set; }
    }
}
