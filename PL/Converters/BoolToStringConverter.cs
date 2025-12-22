using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                string param = parameter as string;
                if (!string.IsNullOrEmpty(param))
                {
                    string[] values = param.Split(';');
                    if (values.Length >= 2)
                    {
                        return boolValue ? values[0] : values[1];
                    }
                    else
                    {
                        return boolValue ? param : "False";
                    }
                }
                return boolValue ? "True" : "False";
            }
            return "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not implemented for BoolToStringConverter.");
        }
    }
}