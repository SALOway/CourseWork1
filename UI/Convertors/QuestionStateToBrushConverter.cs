using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UI.Enums;

namespace UI.Convertors;

public class QuestionStateToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var state = (QuestionState)value;

        return state switch
        {
            QuestionState.None => Brushes.LightGray,
            QuestionState.Selected => Brushes.LightBlue,
            QuestionState.Answered => Brushes.LightGreen,
            _ => throw new NotImplementedException()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}