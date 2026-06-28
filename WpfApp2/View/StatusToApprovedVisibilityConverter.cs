using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BrickByBrick.Models;

namespace BrickByBrick.View
{
    /// <summary>
    /// Converts a ProposalStatus into Visibility based on whether it's Approved.
    /// Pass ConverterParameter="Invert" to flip the logic (visible when NOT approved) —
    /// used for the "not approved yet" placeholder message.
    /// </summary>
    public class StatusToApprovedVisibilityConverter : IValueConverter
    {
        public static readonly StatusToApprovedVisibilityConverter Instance = new StatusToApprovedVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isApproved = value is ProposalStatus status && status == ProposalStatus.Approved;

            bool invert = parameter is string paramText && paramText.Equals("Invert", StringComparison.OrdinalIgnoreCase);

            if (invert)
            {
                isApproved = !isApproved;
            }

            return isApproved ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
