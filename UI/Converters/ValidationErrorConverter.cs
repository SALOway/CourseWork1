using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace UI.Converters;

public class ValidationErrorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        if (value is IEnumerable<ValidationError> errors && errors.Any())
        {
            return errors.Last().ErrorContent;
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
    {
        throw new NotImplementedException();
    }
}
