using System;
using System.Globalization;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts an integer percent (0-100) into a pixel width for a progress bar fill,
    /// scaled against a fixed track width. Track width here matches the 340px-wide
    /// project card minus its padding (approximated at 300px usable space).
    /// </summary>
    public class PercentToWidthConverter : IValueConverter
    {
        public static readonly PercentToWidthConverter Instance = new PercentToWidthConverter();

        private const double TrackWidth = 300.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int percent)
            {
                var clamped = Math.Max(0, Math.Min(100, percent));
                return TrackWidth * (clamped / 100.0);
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
