using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class EditingTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditing)
            {
                return isEditing ? "Редактировать автомобиль" : "Добавить автомобиль";
            }
            return "Автомобиль";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}