using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Inverse of BoolToVisibilityConverter: true -> Collapsed, false -> Visible.
    /// Useful for "empty state" placeholders that should show only when
    /// a bound bool (e.g. HasSelectedProject) is false.
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public static readonly InverseBoolToVisibilityConverter Instance = new InverseBoolToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? Visibility.Collapsed : Visibility.Visible;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
