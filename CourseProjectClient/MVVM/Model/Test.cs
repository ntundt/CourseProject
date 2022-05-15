using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.Model
{
    internal class Test
    {
        public int Id { get; set; }
        public string Title { get => "Тест " + Id; }
        public int AuthorId { get; set; }
        public bool CanNotReviewQuestions { get; set; }
        public TimeSpan TimeLimit { get; set; }
        public DateTime Created { get; set; }
        public DateTime PrivateUntil { get; set; }
        public DateTime PublicUntil { get; set; }
        public int QuestionCount { get; set; }
        public int Attempts { get; set; }
    }
}
