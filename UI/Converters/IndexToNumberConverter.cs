using System.Globalization;
using System.Windows.Data;

namespace UI.Converters;

public class IndexToNumberConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var index = (int)value + 1;
        return index;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}