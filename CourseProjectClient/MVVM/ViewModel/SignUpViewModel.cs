using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.View;
using CourseProjectClient.Services;
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class SignUpViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public ICommand SignUpCommand { get; set; }
        public ICommand GoToLogIn { get; set; }

        public SignUpViewModel()
        {
            GoToLogIn = new RelayCommand(() => {
                NavigationMediator.SetRootViewModel(new LogInViewModel());
            });
            
            SignUpCommand = new RelayCommand(() => {
                try {
                    AuthResult result = Task.Run<AuthResult>(async () => await CommunicationService.GetAuth(_name)).Result;
                    AuthenticationProvider.GetInstance().Apply(result);
                    NavigationMediator.SetRootViewModel(new TestListViewModel());
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            });
        }
    }
}
