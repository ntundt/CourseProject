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
