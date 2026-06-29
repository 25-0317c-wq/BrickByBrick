using System;
using System.Globalization;
using System.Windows.Data;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts a byte count (long) into a human-readable size string,
    /// e.g. 245760 -> "240 KB". Used on the document list in ProjectDetailView.
    /// </summary>
    public class FileSizeFormatConverter : IValueConverter
    {
        public static readonly FileSizeFormatConverter Instance = new FileSizeFormatConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                if (bytes >= 1024 * 1024)
                    return $"{bytes / (1024.0 * 1024.0):0.0} MB";
                if (bytes >= 1024)
                    return $"{bytes / 1024.0:0} KB";
                return $"{bytes} B";
            }

            return "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
