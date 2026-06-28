using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts an integer count into Visibility: 0 -> Visible (show empty-state
    /// message), anything else -> Collapsed. Used for "No team assigned yet."
    /// style placeholders.
    /// </summary>
    public class ZeroCountToVisibilityConverter : IValueConverter
    {
        public static readonly ZeroCountToVisibilityConverter Instance = new ZeroCountToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
