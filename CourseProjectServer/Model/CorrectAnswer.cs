using DataTransferObject;

namespace CourseProjectServer.Model
{
    public class CorrectAnswer
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = "";
        public QuestionType QuestionType { get; set; }
        public CheckAlgorithm CheckAlgorithm { get; set; }
        public double Mark { get; set; }
        public double MaxMark { get; set; }
        public List<CorrectAnswerOption> Options { get; set; } = new();
    }
}
