using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace CompanyApp.Wpf.Converters;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return string.Empty;
        }

        var field = value.GetType().GetField(value.ToString() ?? string.Empty);
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
