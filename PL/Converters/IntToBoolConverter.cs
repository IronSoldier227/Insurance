using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && parameter is string paramStr && int.TryParse(paramStr, out int paramValue))
            {
                return intValue == paramValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue && parameter is string paramStr && int.TryParse(paramStr, out int paramValue))
            {
                return paramValue;
            }
            return 12;
        }
    }
}
