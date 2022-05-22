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
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    public enum PageSelected
    {
        Passed,
        My
    }

    class TestListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ICommand GoToUserSettings { get; set; }

        private PageSelected _selectedPage = PageSelected.Passed;
        public PageSelected SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedPage)));
                if (_selectedPage == PageSelected.My)
                {
                    CurrentViewModel = new MyTestsViewModel();
                }
                if (_selectedPage == PageSelected.Passed)
                {
                    CurrentViewModel = new PassedTestsViewModel();
                }
            }
        }

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
            }
        }

        public TestListViewModel()
        {
            SelectedPage = PageSelected.Passed;

            GoToUserSettings = new RelayCommand(() =>
            {
                NavigationMediator.SetRootViewModel(new UserSettingsViewModel());
            });
        }
    }
}
