using DataTransferObject;

namespace CourseProjectServer.Model
{
    public class Question
    {
        public int QuestionId { get; set; }
        public Test Test { get; set; } = new();
        public int Index { get; set; }
        public string Text { get; set; } = "";
        public QuestionType QuestionType { get; set; }
        public CheckAlgorithm? ChechAlgorithm { get; set; }
        public List<AnswerOption>? AnswerOptions { get; set; }
    }
    public class AnswerOption
    {
        public int AnswerOptionId { get; set; }
        public string Text { get; set; } = "";
        public Question Question { get; set; } = new();
        public bool IsCorrect { get; set; }
        public bool? Checked { get; set; }
    }
}
