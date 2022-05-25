namespace DataTransferObject
{
    public class CorrectAnswerOptionInfo
    {
        // id варианта ответа
        public int AnswerId { get; set; }
        // Текст варианта ответа
        public string Text { get; set; } = "";
        // Выбрал ли пользователь этот вариант ответа
        public bool Checked { get; set; }
        // Является ли этот вариант ответа правильным на самом деле
        public bool IsActuallyCorrect { get; set; }
    }
}