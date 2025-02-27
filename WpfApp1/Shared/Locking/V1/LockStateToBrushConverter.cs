using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Shared.Locking.V1;

/// <summary>
/// Enhanced converter for displaying lock state as a brush color
/// </summary>
public class LockStateToBrushConverter : IValueConverter
{
    /// <summary>
    /// Color when locked by another user
    /// </summary>
    public Brush LockedByOtherBrush { get; set; } = Brushes.LightCoral;

    /// <summary>
    /// Color when locked by the current user
    /// </summary>
    public Brush LockedByMeBrush { get; set; } = Brushes.LightGreen;

    /// <summary>
    /// Color when a lock has expired
    /// </summary>
    public Brush LockExpiredBrush { get; set; } = Brushes.Orange;

    /// <summary>
    /// Color when unlocked (no lock)
    /// </summary>
    public Brush UnlockedBrush { get; set; } = Brushes.Transparent;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockState lockState)
        {
            return lockState switch
            {
                LockState.LockedByOther => LockedByOtherBrush,
                LockState.LockedByMe => LockedByMeBrush,
                LockState.LockExpired => LockExpiredBrush,
                LockState.Unlocked => UnlockedBrush,
                _ => UnlockedBrush
            };
        }
        return UnlockedBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}