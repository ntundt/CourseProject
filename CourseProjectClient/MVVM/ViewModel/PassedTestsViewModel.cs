using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
using CourseProjectClient.MVVM.View;
using CourseProjectClient.Services;
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class PassedTestsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private ObservableCollection<Attempt> _passedTests;
        public ObservableCollection<Attempt> PassedTests
        {
            get => _passedTests;
            set
            {
                _passedTests = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(PassedTests)));
                
            }
        }

        public ICommand SeeResults { get; set; }


        private string _testId;
        public string TestId
        {
            get => _testId;
            set
            {
                _testId = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TestId)));
            } 
        }

        public ICommand StartAttempt { get; set; }

        private void RetrievePassedTests()
        {
            try
            {
                List<AttemptInfo> result = Task.Run<List<AttemptInfo>>(async () => await CommunicationService.GetAttemptInfos()).Result;
                _passedTests = new ObservableCollection<Attempt>(result.Select(x => new Attempt
                {
                    Id = x.AttemptId,
                    TestId = x.TestId,
                    UserId = x.UserId,
                    Started = DateTimeOffset.FromUnixTimeSeconds(x.Started).DateTime,
                    Ended = DateTimeOffset.FromUnixTimeSeconds(x.Ended ?? 0).DateTime
                }));
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }

        public PassedTestsViewModel()
        {
            RetrievePassedTests();

            SeeResults = new RelayCommand((id) =>
            {
                if (!(id is int))
                {
                    return;
                }
                AttemptResultViewModel page = new AttemptResultViewModel();
                page.SetAttemptId((int)id);
                NavigationMediator.SetRootViewModel(page);
            });
            StartAttempt = new RelayCommand(() =>
            {
                try
                {
                    GetStartTestResult result = 
                        Task.Run<GetStartTestResult>(async () => await CommunicationService.StartTest(Convert.ToInt32(_testId))).Result;

                    var model = new TestViewModel();
                    var attempt = new Attempt
                    {
                        Id = result.AttemptId,
                        UserId = AuthenticationProvider.GetInstance().UserId,
                        TestId = Convert.ToInt32(_testId),
                        Started = DateTimeOffset.FromUnixTimeSeconds(result.Started).DateTime,
                        Ended = DateTimeOffset.FromUnixTimeSeconds(result.Limit ?? 0).DateTime
                    };
                    model.Attempt = attempt;
                    NavigationMediator.SetRootViewModel(model);
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }, () =>
            {
                try
                {
                    return Convert.ToInt32(_testId) > 0;
                }
                catch (FormatException)
                {
                    return false;
                }
            });
        }
    }
}
