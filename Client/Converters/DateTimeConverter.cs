namespace Client.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DateTimeConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
            {
                return null;
            }

            return dateTime >= DateTime.Today && dateTime <= DateTime.Today + TimeSpan.FromDays(1) ? $"{dateTime:HH:mm}" : $"{dateTime:dd.MM.yy}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
