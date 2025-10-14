using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using FinancialLitApp.Models;

namespace FinancialLitApp.Converters
{
    public class CategoryFilterConverter : IValueConverter
    {
        public string CategoryFilter { get; set; }
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture)
        {
            //here i'm converting items to filtered by category:
            if(value is itemCategory category && !string.IsNullOrEmpty(CategoryFilter))
            {
                return category.ToString().Equals(CategoryFilter, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value , Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
