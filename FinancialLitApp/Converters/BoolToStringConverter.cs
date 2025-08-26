using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        // this converter converts the bool value to string pair and return the what's inside the pair from index o or checks the quantity of string pairs(has te be more than 1) before returning an empty string.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        {
            if(value is bool boolValue && parameter is string strings)
            {
                var stringPair = strings.Split('|');
                return boolValue ? stringPair[0] : (stringPair.Length > 1 ? stringPair[1] : string.Empty);
            }
            return string.Empty;
        }

        public object ConvertBack(object value , Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
       
    }
}
