using System;
using System.Globalization;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts IsActive (bool) into a display string: "Active" / "Inactive".
    /// </summary>
    public class BoolToStatusTextConverter : IValueConverter
    {
        public static readonly BoolToStatusTextConverter Instance = new BoolToStatusTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
                return isActive ? "Active" : "Inactive";

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}