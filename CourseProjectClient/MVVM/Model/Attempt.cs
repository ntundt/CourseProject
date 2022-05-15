using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.Model
{
    internal class Attempt : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private int _id;
        public int Id 
        { 
            get => _id; 
            set
            {
                _id = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Id)));
            } 
        }

        private int _testId;
        public int TestId
        {
            get => _testId;
            set
            {
                _testId = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TestId)));
            }
        }

        private int _userId;
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(UserId)));
            }
        }
        public int? Mark { get; set; }
        public int? MaxMark { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Ended { get; set; }
    }
}
