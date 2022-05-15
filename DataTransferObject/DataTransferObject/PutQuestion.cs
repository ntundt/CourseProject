using System.Collections.Generic;

namespace DataTransferObject
{
    public class PutQuestion
    {
        public int? Index { get; set; }
        public int? QuestionType { get; set; }
        public int? CheckAlgorithm { get; set; }
        public string? Text { get; set; }
        public List<PutAnswerOption>? AnswerOptions { get; set; }
    }
    public class PutAnswerOption
    {
        public int? AnswerId { get; set; }
        public string? Text { get; set; }
        public bool? Checked { get; set; }
    }
}
