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
            // i commnted this section out bc there was an issue with how the color strings are converted into colors bc of the need to trim white spaces 
            //if(value is  bool isSelected && parameter is string colors)
            //{
            //    var colorPair = colors.Split('|');
            //    System.Diagnostics.Debug.WriteLine($"   Split result: [{string.Join("', '", colorPair)}]");
            //    string selectedColor = colorPair[0]?.Trim(); // trimming the write spaces
            //    string unselectedColor = colorPair.Length > 1 ? colorPair[1]?.Trim() : "#808080";
            //    return isSelected ? Color.FromArgb(colorPair[0]) : Color.FromArgb(colorPair[1]);
            //    System.Diagnostics.Debug.WriteLine($"   Selected color: '{selectedColor}'");
            //    System.Diagnostics.Debug.WriteLine($"   Unselected color: '{unselectedColor}'");

            //}
           //wrote a more detiled broken down version of what's going.
            if (value is bool isSelected && parameter is string colors)
            {
                System.Diagnostics.Debug.WriteLine($"   IsSelected: {isSelected}");
                System.Diagnostics.Debug.WriteLine($"   Colors string: '{colors}'");

                var colorPair = colors.Split('|');
                System.Diagnostics.Debug.WriteLine($"   Split result: [{string.Join("', '", colorPair)}]");

                // Clean the color strings by trimming whitespace so the vonersion works well to show the actual color.
                string selectedColor = colorPair[0]?.Trim();
                string unselectedColor = colorPair.Length > 1 ? colorPair[1]?.Trim() : "#808080";

                System.Diagnostics.Debug.WriteLine($"   Selected color: '{selectedColor}'");
                System.Diagnostics.Debug.WriteLine($"   Unselected color: '{unselectedColor}'");

                try
                {
                    // Test each color individually
                    Color testSelected = Color.FromArgb(selectedColor);
                    System.Diagnostics.Debug.WriteLine($"   ✅ Selected color parsed successfully: {testSelected}");

                    Color testUnselected = Color.FromArgb(unselectedColor);
                    System.Diagnostics.Debug.WriteLine($"   ✅ Unselected color parsed successfully: {testUnselected}");

                    var result = isSelected ? testSelected : testUnselected;
                    System.Diagnostics.Debug.WriteLine($"   🎯 Returning: {result}");
                    return result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"   ❌ Color conversion error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"   ❌ Stack trace: {ex.StackTrace}");
                    System.Diagnostics.Debug.WriteLine($"   ❌ Attempted colors: '{selectedColor}' | '{unselectedColor}'");

                    // Return default colors on error
                    return isSelected ? Colors.Blue : Colors.Gray;
                }
            }

            System.Diagnostics.Debug.WriteLine($"   ⚠️ Fallback to Gray - conditions not met");
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type target, object parameter, CultureInfo culture)// this is the other method that the "IValue Interface" needs.
        {
            throw new NotImplementedException(); // returns the target back to the source, oerfoming the opposite conversion to the "Convert" method above
        }
    }
}
