using CourseProjectClient.Exceptions;
using CourseProjectClient.Services;
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class AttemptResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ICommand OkCommand { get; set; }

        private ResultInfo _resultInfo;
        public ResultInfo ResultInfo
        {
            get => _resultInfo;
            set
            {
                _resultInfo = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ResultInfo)));
            }
        }

        public ObservableCollection<CorrectAnswerInfo> Questions = new ObservableCollection<CorrectAnswerInfo>();

        public void SetAttemptId(int id)
        {
            try
            {
                ResultInfo = Task.Run<ResultInfo>(async () => await CommunicationService.GetResultInfo(id)).Result;
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            } 
            catch (AggregateException e) when (e.InnerException is DefaultException) 
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }
        public AttemptResultViewModel()
        {
            OkCommand = new RelayCommand(() =>
            {
                NavigationMediator.SetRootViewModel(new TestListViewModel());
            });
        }
    }
}
