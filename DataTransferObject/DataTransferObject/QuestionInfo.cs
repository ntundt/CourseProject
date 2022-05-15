using System.Collections.Generic;

namespace DataTransferObject
{
    public class QuestionInfo
    {
        public int QuestionId { get; set; }
        public int Index { get; set; }
        public QuestionType QuestionType { get; set; }
        public CheckAlgorithm? CheckAlgorithm { get; set; }
        public string Text { get; set; } = "";
        public List<AnswerOptionInfo> AnswerOptions { get; set; } = new List<AnswerOptionInfo>();
        public QuestionInfo() { }
    }
    public class AnswerOptionInfo
    {
        public int AnswerId { get; set; }
        public string Text { get; set; } = "";
        public bool? Checked { get; set; }
        public AnswerOptionInfo() { }
    }
}
