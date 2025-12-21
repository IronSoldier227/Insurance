// PL/Converters/InsuranceToIdConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;
using Interfaces.DTO;

namespace PL.Converters
{
    public class InsuranceToIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Insurance insurance)
                return insurance.Id;
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}