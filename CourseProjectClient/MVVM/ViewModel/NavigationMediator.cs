using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal static class NavigationMediator
    {
        public static event Action<object> CurrentViewModelUpdated = delegate { };
        public static void UpdateCurrentViewModel(object viewModel)
        {
            CurrentViewModelUpdated(viewModel);
        }

        public static event Action CurrentViewModelClosed = delegate { };
        public static void CloseCurrentViewModel()
        {
            CurrentViewModelClosed();
        }

        public static event Action<object> RootViewModelSet = delegate { };
        public static void SetRootViewModel(object viewModel)
        {
            RootViewModelSet(viewModel);
        }

        public static event Action<Page> CurrentPageUpdated = delegate { };
        public static void UpdateCurrentPage(Page page)
        {
            CurrentPageUpdated(page);
        }

        public static event Action CurrentPageClosed = delegate { };
        public static void CloseCurrentPage()
        {
            CurrentPageClosed();
        }

        public static event Action<Page> RootPageSet = delegate { };
        public static void SetRootPage(Page page)
        {
            RootPageSet(page);
        }
    }
}
