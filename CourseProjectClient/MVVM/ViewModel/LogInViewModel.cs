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
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class LogInViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Login)));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Password)));
            }
        }

        public ICommand LogInCommand { get; set; }

        public ICommand GoToSignUpCommand { get; set; }

        public LogInViewModel()
        {
            GoToSignUpCommand = new RelayCommand(() =>
            {
                NavigationMediator.SetRootViewModel(new SignUpViewModel());
            });

            LogInCommand = new RelayCommand(() =>
            {
                try
                {
                    AuthResult result = Task.Run<AuthResult>(async () => await CommunicationService.GetAuth(_login, _password)).Result;
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
