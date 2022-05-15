using System.Collections.Generic;

namespace DataTransferObject {
    public class ResultInfo {
        public int AttemptId { get; set; }
        public int UserId { get; set; }
        public int TestId { get; set; }
        public double Mark { get; set; }
        public double MaxMark { get; set; }
        public long Started { get; set; }
        public long Ended { get; set; }
        public List<CorrectAnswerInfo> Answers { get; set; } = new List<CorrectAnswerInfo>();
    }
}