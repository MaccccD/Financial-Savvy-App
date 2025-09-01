using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FinancialLitApp.Converters // the namespace that directs where the converter is stored
{
    public class BoolToColorConverter : IValueConverter // binding converter class  interface requires both a "convert" method  and a "ConvertBack" method.
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // data is moving from one source to the next target in one way or two way bindings  
        {
            if(value is  bool isSelected && parameter is string colors)
            {
                var colorPair = colors.Split('|');
                return isSelected ? Color.FromArgb(colorPair[0]) : Color.FromArgb(colorPair[1]);
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type target, object parameter, CultureInfo culture)// this is the other method that the "IValue Interface" needs.
        {
            throw new NotImplementedException(); // returns the target back to the source, oerfoming the opposite conversion to the "Convert" method above
        }
    }
}
