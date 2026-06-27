using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts a bool into Visibility.Visible / Visibility.Collapsed.
    /// Used to show/hide modal overlays (e.g. the Add User dialog) based on a
    /// ViewModel flag like IsAddUserDialogOpen.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Instance = new BoolToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
                return isVisible ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;

            return false;
        }
    }
}
