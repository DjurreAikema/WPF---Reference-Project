using System.Globalization;
using System.Windows.Data;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Converter to display lock state as text
/// </summary>
public class LockStateToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockState lockState)
        {
            return lockState switch
            {
                LockState.LockedByOther => "Locked by another user",
                LockState.LockedByMe => "You have locked this item",
                LockState.LockExpired => "Lock has expired",
                LockState.Unlocked => "Not locked",
                _ => "Unknown state"
            };
        }
        return "Unknown state";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}