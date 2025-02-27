using System.Globalization;
using System.Windows.Data;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Converter to transform a lock state to visibility
/// </summary>
public class LockStateToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// If true, the converter will return visible for the specified state and collapsed for others
    /// If false, it will return collapsed for the specified state and visible for others
    /// </summary>
    public bool ShowForState { get; set; } = true;

    /// <summary>
    /// The lock state that will trigger visibility
    /// </summary>
    public LockState TargetState { get; set; } = LockState.LockedByMe;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockState lockState)
        {
            bool isMatch = lockState == TargetState;
            return (ShowForState == isMatch) ?
                System.Windows.Visibility.Visible :
                System.Windows.Visibility.Collapsed;
        }
        return System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}