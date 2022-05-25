using System.Collections.Generic;

namespace DataTransferObject {
    public class ResultInfo 
    {
        // id �������
        public int AttemptId { get; set; }
        // id ������������, ������������ ����
        public int UserId { get; set; }
        // ��� ������������, ������������ ����
        public string UserName { get; set; }
        // id �����, ������� ����������
        public int TestId { get; set; }
        // ������ �� ���� 
        public double Mark { get; set; }
        // ������������ ������ �� ����
        public double MaxMark { get; set; }
        // ����� ������ ������� (unix timestamp, �������)
        public long Started { get; set; }
        // ����� ���������� ������� (unix timestamp, �������)
        public long Ended { get; set; }
        // ������ ���������� ������� �� �������
        public List<CorrectAnswerInfo> Answers { get; set; } = new List<CorrectAnswerInfo>();
    }
}