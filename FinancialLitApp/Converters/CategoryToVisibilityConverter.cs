using System;
using System.Collections.Generic;
using System.Globalization;
using FinancialLitApp.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialLitApp.Converters
{
    public  class CategoryToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ItemCategory category && parameter is string targetCategory)
            {
                return category.ToString().Equals(targetCategory, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
