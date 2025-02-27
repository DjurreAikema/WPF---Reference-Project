using System.Globalization;
using System.Windows.Data;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Converter to format lock time remaining
/// </summary>
public class LockTimeRemainingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime expiryTime)
        {
            if (expiryTime <= DateTime.UtcNow)
                return "Expired";

            var timeRemaining = expiryTime - DateTime.UtcNow;

            if (timeRemaining.TotalHours >= 1)
                return $"{Math.Floor(timeRemaining.TotalHours)}h {timeRemaining.Minutes}m remaining";
            else if (timeRemaining.TotalMinutes >= 1)
                return $"{timeRemaining.Minutes}m {timeRemaining.Seconds}s remaining";
            else
                return $"{timeRemaining.Seconds}s remaining";
        }

        return "No lock";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}