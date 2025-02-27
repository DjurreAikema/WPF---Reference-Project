using System.Globalization;
using System.Windows.Data;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Converter to determine if an item is editable based on lock state
/// </summary>
public class LockStateToEditableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockState lockState)
        {
            // Only editable if locked by the current user or unlocked
            return lockState == LockState.LockedByMe || lockState == LockState.Unlocked;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}