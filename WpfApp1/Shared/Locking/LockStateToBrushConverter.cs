using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Shared.Locking;

public class LockStateToBrushConverter : IValueConverter
{
    public Brush LockedBrush { get; set; } = Brushes.Red;
    public Brush SoftLockedBrush { get; set; } = Brushes.Yellow;
    public Brush UnlockedBrush { get; set; } = Brushes.Transparent;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LockState lockState)
        {
            return lockState switch
            {
                LockState.Locked => LockedBrush,
                LockState.SoftLocked => SoftLockedBrush,
                LockState.Unlocked => UnlockedBrush,
                _ => UnlockedBrush
            };
        }
        return UnlockedBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}