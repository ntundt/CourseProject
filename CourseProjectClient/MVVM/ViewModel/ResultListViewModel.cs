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
    internal class ResultListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ICommand SeeMore { get; set; }

        public ICommand GoBack { get; set; }

        private ObservableCollection<ResultInfo> _results;
        public ObservableCollection<ResultInfo> Results
        { 
            get => _results;
            set
            {
                _results = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Results)));
            }
        }

        public void SetTestId(int id)
        {
            try
            {
                Results = new ObservableCollection<ResultInfo>(
                    Task.Run<List<ResultInfo>>(async () => await CommunicationService.GetTestResultsInfo(id)).Result
                );
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            } 
        }

        public ResultListViewModel()
        {
            SeeMore = new RelayCommand((id) =>
            {
                AttemptResultViewModel model = new AttemptResultViewModel();
                model.SetAttemptId((int)id);
                NavigationMediator.SetRootViewModel(model);
            });

            GoBack = new RelayCommand(() =>
            {
                NavigationMediator.SetRootViewModel(new TestListViewModel());
            });
        }
    }
}
