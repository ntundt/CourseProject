using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class ChangePageCommand : ICommand
    {
        private Page _target;
        private bool _canExecute;
        public ChangePageCommand(Page target)
        {
            _target = target;
        }
        public bool CanExecute(object obj)
        {
            return _canExecute;
        }
        public void SetCanExecute(bool canExecute)
        {
            _canExecute = canExecute;
            CanExecuteChanged(this, new EventArgs());
        }
        public void Execute(object obj)
        {

        }
        public event EventHandler CanExecuteChanged = delegate { };
    }
}
