using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts a local file path (string) into a BitmapImage for binding
    /// directly to an Image control's Source. Used to display the QuickChart
    /// PNG saved by QuickChartService. Reloads fresh from disk each time
    /// (no caching) so a newly-rendered chart replacing an old one is always
    /// picked up rather than showing a stale cached image.
    /// </summary>
    public class FilePathToImageConverter : IValueConverter
    {
        public static readonly FilePathToImageConverter Instance = new FilePathToImageConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriCachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
