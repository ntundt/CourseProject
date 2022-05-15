using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.MVVM.ViewModel
{
    class TestListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TestInfo> _tests;
        public ObservableCollection<TestInfo> Tests
        {
            get => _tests;
            set
            {
                _tests = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Tests)));
            }
        }

        public TestListViewModel()
        {
            _tests = new ObservableCollection<TestInfo>()
            {
                new TestInfo()
                {
                    Id=1,
                    AuthorId=12,
                    TimeLimit=3600
                }
            };
        }
    }
}
