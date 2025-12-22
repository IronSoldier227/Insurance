// PL/Converters/BoolToStringConverter.cs
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
                // Разбиваем параметр на две строки: для True и для False
                string param = parameter as string;
                if (!string.IsNullOrEmpty(param))
                {
                    string[] values = param.Split(';');
                    if (values.Length >= 2)
                    {
                        return boolValue ? values[0] : values[1];
                    }
                    // Если разделитель не найден, используем весь параметр для True, и "False" для False
                    else
                    {
                        return boolValue ? param : "False";
                    }
                }
                // Если параметр не задан, используем стандартные значения
                return boolValue ? "True" : "False";
            }
            return "False"; // Значение по умолчанию, если тип не bool
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Обратное преобразование не требуется для отображения
            throw new NotImplementedException("ConvertBack is not implemented for BoolToStringConverter.");
        }
    }
}