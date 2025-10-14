using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using FinancialLitApp.Models;
namespace FinancialLitApp.Converters
{
    public class CategoryToColorConverter : IValueConverter
    {
        public object Convert(object value , Type targetType, object parameter, CultureInfo culture)
        {
            if(value is itemCategory category)
            {
                // this converts is about getting the color based on a  category :
                return category switch
                {
                    itemCategory.Need => Color.FromArgb("#27ae60"),
                    itemCategory.Want => Color.FromArgb("#f39c12"),
                    itemCategory.ImpulsePurchase => Color.FromArgb("#e74c3c"),
                    itemCategory.Investment => Color.FromArgb("#9b59b6"),
                    _ => Colors.Gray
                };
                
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value,  Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
