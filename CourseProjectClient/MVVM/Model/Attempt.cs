using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.Model
{
    public class Attempt : INotifyPropertyChanged
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

        private DateTime _started;
        public DateTime Started
        {
            get => _started;
            set
            {
                _started = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Started)));
            }
        }


        private DateTime? _ended;
        public DateTime? Ended
        {
            get => _ended;
            set
            {
                _ended = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Ended)));
            }
        }

        public bool HasEnded
        {
            get => (Ended ?? Started) > Started;
        }
    }
}
