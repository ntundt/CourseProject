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
            NavigationMediator.CurrentPageUpdated += (page) => CurrentPage = page;
            NavigationMediator.CurrentPageClosed += () => GoBack();
            NavigationMediator.RootPageSet += (page) => SetRootPage(page);
            
            if (AuthenticationProvider.GetInstance().ReadFromFile())
            {
                TestListView testList = new TestListView();
                NavigationMediator.SetRootPage(testList);
                return;
            } 

            SignUp signup = new SignUp();
            NavigationMediator.SetRootPage(signup);
        }
    }
}
