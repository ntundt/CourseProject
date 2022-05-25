namespace DataTransferObject
{
    public class CorrectAnswerOptionInfo
    {
        // id �������� ������
        public int AnswerId { get; set; }
        // ����� �������� ������
        public string Text { get; set; } = "";
        // ������ �� ������������ ���� ������� ������
        public bool Checked { get; set; }
        // �������� �� ���� ������� ������ ���������� �� ����� ����
        public bool IsActuallyCorrect { get; set; }
    }
}