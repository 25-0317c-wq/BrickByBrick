using System;
using System.Globalization;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts a full name string (e.g. "Carlo Santos") into initials ("CS")
    /// for small avatar badges. Used on the Project Detail View's team list.
    /// </summary>
    public class NameToInitialsConverter : IValueConverter
    {
        public static readonly NameToInitialsConverter Instance = new NameToInitialsConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string name && !string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
                }
                if (parts.Length >= 2)
                {
                    return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
                }
            }

            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
