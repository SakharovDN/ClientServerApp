namespace Client.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    using Common;

    internal class IconConverter : IMultiValueConverter
    {
        #region Methods

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is ChatTypes chatType))
            {
                return null;
            }

            if (!(values[1] is bool isSelected))
            {
                return null;
            }

            string iconPath = isSelected
                                  ? Environment.CurrentDirectory + $"/Images/{chatType}Selected.png"
                                  : Environment.CurrentDirectory + $"/Images/{chatType}NotSelected.png";

            try
            {
                return new BitmapImage(new Uri(iconPath));
            }
            catch
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
