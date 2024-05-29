using Core.Enums;
using System.Globalization;
using System.Windows.Data;

namespace UI.Convertors;

public class AttemptStatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "-";
        }

        var testAttemptStatus = (TestAttemptStatus)value;
        return testAttemptStatus switch
        {
            TestAttemptStatus.InProcess => "In process",
            TestAttemptStatus.Completed => "Completed",
            TestAttemptStatus.Expired => "Expired",
            TestAttemptStatus.Failed => "Failed",
            TestAttemptStatus.Cancelled => "Canceled",
            _ => "-"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
