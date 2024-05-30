using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace UI.Converters;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString())) return null;
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (FieldInfo fi in targetType.GetFields())
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes.Length > 0) && (attributes[0].Description == (string)value))
                return Enum.Parse(targetType, fi.Name);
        }
        return Enum.Parse(targetType, (string)value);
    }
}