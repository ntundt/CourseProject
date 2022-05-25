using System.Collections.Generic;

namespace DataTransferObject {
    public class CorrectAnswerInfo {
        // id вопроса, правильный ответ на который дается
        public int QuestionId { get; set; }
        // Порядковый номер вопроса
        public int Index { get; set;}
        // Текст вопроса
        public string Text { get; set; } = "";
        // Оценка за вопрос
        public double Mark { get; set; }
        // Максимальная оценка за вопрос
        public double MaxMark { get; set; }
        // Алгоритм проверки ответа на вопрос с множественным выбором
        public int CheckAlgorithm { get; set; }
        // Вид вопроса
        public int QuestionType { get; set; }
        // Список правильных вариантов ответа
        public List<CorrectAnswerOptionInfo> Options { get; set; } = new List<CorrectAnswerOptionInfo>();
    }
}
        