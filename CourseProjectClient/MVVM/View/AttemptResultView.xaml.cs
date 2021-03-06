using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseProjectClient.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для AttemptResultView.xaml
    /// </summary>
    public partial class AttemptResultView : UserControl
    {
        public AttemptResultView()
        {
            InitializeComponent();
        }
        
        private void WrapPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollView.ScrollToVerticalOffset(ScrollView.VerticalOffset - e.Delta);
        }
    }
}
