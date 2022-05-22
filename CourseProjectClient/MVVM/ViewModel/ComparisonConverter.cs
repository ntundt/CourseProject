using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CourseProjectClient.MVVM.ViewModel
{
    public class ComparisonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.Equals(typeof(Visibility)))
            {
                return (parameter.Equals(value.ToString().ToLower())
                    || parameter.Equals(value)
                    || (value?.Equals(parameter) ?? false)) ? Visibility.Visible : Visibility.Collapsed;
                
            }
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
