using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
using CourseProjectClient.Services;
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
    internal class MyTestsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<Test> _myTests;
        public ObservableCollection<Test> MyTests
        {
            get => _myTests;
            set
            {
                _myTests = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MyTests)));
            }
        }

        private void RetrieveMyTests()
        {
            try
            {
                GetTestsResult result = Task.Run<GetTestsResult>(async () => await CommunicationService.GetTests()).Result;
                _myTests = new ObservableCollection<Test>(result.Tests.Select(x => new Test
                {
                    Id = x.Id,
                    AuthorId = x.AuthorId,
                    CanNotReviewQuestions = x.CanNotReviewQuestions,
                    TimeLimit = new TimeSpan(0, 0, (int)x.TimeLimit),
                    Created = DateTimeOffset.FromUnixTimeSeconds(x.CreatedDate).DateTime,
                    PrivateUntil = DateTimeOffset.FromUnixTimeSeconds(x.PrivateUntil).DateTime,
                    PublicUntil = DateTimeOffset.FromUnixTimeSeconds(x.PublicUntil).DateTime,
                    Attempts = x.Attempts
                }));
            }
            catch (DefaultException e)
            {
                e.ShowSnackBar();
            }
        }

        public MyTestsViewModel()
        {
            RetrieveMyTests();
        }
    }
}
