using CourseProjectClient.MVVM.View;
using CourseProjectClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFUI.Controls;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class MainWindow : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private Stack<object> _viewModels = new Stack<object>();
        public object CurrentViewModel
        {
            get => _viewModels.Peek();
        }

        private Stack<Page> _pages = new Stack<Page>();
        public Page CurrentPage
        {
            get => _pages.Peek() as Page;
            set
            {
                _pages.Push(value);
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
            }
        }


        public void SetRootViewModel(object vm)
        {
            _viewModels.Clear();
            _viewModels.Push(vm);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
        }
        public void GoBackVM()
        {
            _viewModels.Pop();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
        }
        public void ReplaceCurrentViewModel(object vm)
        {
            _viewModels.Pop();
            _viewModels.Push(vm);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentViewModel)));
        }

        public void GoBack() {
            _pages.Pop();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
        }
        public void SetRootPage(Page page)
        {
            _pages.Clear();
            _pages.Push(page);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
        }
        public void ReplaceCurrentPage(Page page)
        {
            _pages.Pop();
            _pages.Push(page);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
        }
        
        public MainWindow()
        {
            NavigationMediator.RootViewModelSet += SetRootViewModel;
            NavigationMediator.CurrentViewModelClosed += GoBackVM;
            NavigationMediator.CurrentViewModelUpdated += ReplaceCurrentViewModel;

            if (AuthenticationProvider.GetInstance().ReadFromFile())
            {
                NavigationMediator.SetRootViewModel(new TestListViewModel());
                return;
            }

            NavigationMediator.SetRootViewModel(new SignUpViewModel());
        }
    }
}
