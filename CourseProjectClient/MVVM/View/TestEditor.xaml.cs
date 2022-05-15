using CourseProjectClient.MVVM.ViewModel;
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
    /// Логика взаимодействия для TestEditor.xaml
    /// </summary>
    public partial class TestEditor : Page
    {
        public TestEditor()
        {
            InitializeComponent();
        }

        private void WrapPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            sidebar.ScrollToVerticalOffset(sidebar.VerticalOffset - e.Delta);
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as TestEditViewModel).SetSelectedQuestion(Convert.ToInt32(((sender as Border).Child as TextBlock).Text));
        }
    }
}
