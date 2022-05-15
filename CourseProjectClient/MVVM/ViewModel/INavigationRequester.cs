using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal delegate void PageChangeRequestHandler(INavigationRequester targetPage);
    internal interface INavigationRequester
    {
        event PageChangeRequestHandler PageChangeRequested;
    }
}
