using System;
using System.Globalization;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Simple boolean negation: true becomes false, false becomes true.
    /// Used for things like Button.IsEnabled bound to the inverse of an
    /// "IsBusy" flag.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public static readonly InverseBoolConverter Instance = new InverseBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return true;
        }
    }
}
