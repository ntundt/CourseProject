using CourseProjectClient.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProjectClient.Exceptions
{
    internal class DefaultException : Exception
    {
        private int _errorCode;
        private string _message;
        public DefaultException(int code, string message)
        {
            _errorCode = code;
            _message = message;
        }

        public void ShowSnackBar()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.RootSnackbar.Show($"Ошибка {_errorCode}", _message);
        }
    }
}
