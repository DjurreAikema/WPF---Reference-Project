using System.Globalization;
using System.Windows.Data;

namespace WpfApp2.Shared.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        private bool Invert { get; set; } = false;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var result = value != null;
            return Invert ? !result : result;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}