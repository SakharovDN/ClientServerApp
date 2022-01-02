using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
            {
                return null;
            }

            if (dateTime >= DateTime.Today && dateTime <= DateTime.Today + TimeSpan.FromDays(1))
            {
                return $"{dateTime:HH:mm}";
            }
            else
            {
                return $"{dateTime:dd.MM.yy}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
