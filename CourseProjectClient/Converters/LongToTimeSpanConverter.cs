using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CourseProjectClient.Converters
{
    class LongToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is long))
            {
                throw new InvalidOperationException();
            }
            return new TimeSpan(0, 0, (int)(long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                throw new InvalidOperationException();
            }
            return (long)((TimeSpan)value).TotalSeconds;
        }
    }
}
