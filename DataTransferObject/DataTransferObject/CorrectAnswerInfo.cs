using System.Collections.Generic;

namespace DataTransferObject {
    public class CorrectAnswerInfo {
        public int QuestionId { get; set; }
        public int Index { get; set;}
        public string Text { get; set; } = "";
        public double Mark { get; set; }
        public double MaxMark { get; set; }
        public int CheckAlgorithm { get; set; }
        public int QuestionType { get; set; }
        public List<CorrectAnswerOptionInfo> Options { get; set; } = new List<CorrectAnswerOptionInfo>();
    }
}
        