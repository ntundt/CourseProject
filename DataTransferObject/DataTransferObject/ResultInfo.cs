using System.Collections.Generic;

namespace DataTransferObject {
    public class ResultInfo 
    {
        // id попытки
        public int AttemptId { get; set; }
        // id пользователя, проходившего тест
        public int UserId { get; set; }
        // Имя пользователя, проходившего тест
        public string UserName { get; set; }
        // id теста, который проходился
        public int TestId { get; set; }
        // Оценка за тест 
        public double Mark { get; set; }
        // Максимальная оценка за тест
        public double MaxMark { get; set; }
        // Время начала попытки (unix timestamp, секунды)
        public long Started { get; set; }
        // Время завершения попытки (unix timestamp, секунды)
        public long Ended { get; set; }
        // Список правильных ответов на вопросы
        public List<CorrectAnswerInfo> Answers { get; set; } = new List<CorrectAnswerInfo>();
    }
}