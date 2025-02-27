using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModernSort.Converters
{
    internal class ValidationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values == Enumerable.Empty<object>()) 
                throw new ArgumentException("Ошибка передачи параметров");

            string result = String.Empty;

            foreach (object value in values)
            {
                if (value is string && !value.Equals(String.Empty))
                {
                    result += result == String.Empty ? $"{value}": $" and {value}";
                 }
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
