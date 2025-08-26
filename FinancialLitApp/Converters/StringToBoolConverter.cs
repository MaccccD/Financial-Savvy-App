using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FinancialLitApp.Converters
{
    public  class StringToBoolConverter : IValueConverter
    {
        public object Convert (object value,  Type targetType, object parameter, CultureInfo culture)
        {
           return !string.IsNullOrEmpty (value.ToString ());
        }
        

        public object ConvertBack(object value, Type target, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}
