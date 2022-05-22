using CourseProjectClient.Exceptions;
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
    internal class UserSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ICommand SaveUserInfoCommand { get; set; }
        public ICommand Cancel { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Login"));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }

        private string _passwordConfirm;
        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set
            {
                _passwordConfirm = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PasswordConfirm"));
            }
        }

        private void RetrieveInfo() 
        {
            try {
                var result = Task.Run(async () => await CommunicationService.GetUserInfo()).Result;

                Name = result.Name;
                Login = result.Login;
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }

        public UserSettingsViewModel()
        {
            RetrieveInfo();

            SaveUserInfoCommand = new RelayCommand(() =>
            {
                try {
                    var result = Task.Run(async () => await CommunicationService.SaveUserInfo(new PutUserInfo
                    {
                        Name = Name,
                        Login = Login,
                        Password = Password,
                    })).Result;

                    NavigationMediator.SetRootViewModel(new TestListViewModel());
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }, () =>
            {
                return !string.IsNullOrWhiteSpace(Name) 
                    && !string.IsNullOrWhiteSpace(Login) 
                    && !string.IsNullOrWhiteSpace(Password) 
                    && Password == PasswordConfirm
                    && Password.Length >= 6
                    && Password.All(char.IsLetterOrDigit)
                    && Login.Length >= 6
                    && Login.All(char.IsLetterOrDigit);
            });

            Cancel = new RelayCommand(() =>
            {
                NavigationMediator.SetRootViewModel(new TestListViewModel());
            });
        }
    }
}
