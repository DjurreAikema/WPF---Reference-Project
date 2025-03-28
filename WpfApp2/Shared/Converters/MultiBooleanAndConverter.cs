using System.Globalization;
using System.Windows.Data;

namespace WpfApp2.Shared.Converters;

public class MultiBooleanAndConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length == 0)
            return false;

        foreach (object value in values)
        {
            if (value is bool boolValue && !boolValue)
                return false;
        }

        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}