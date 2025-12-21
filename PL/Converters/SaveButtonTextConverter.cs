// PL/Converters/SaveButtonTextConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class SaveButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditing)
            {
                return isEditing ? "Обновить" : "Сохранить";
            }
            return "Сохранить";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}