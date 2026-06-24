using System.ComponentModel;
using System.Reflection;

namespace CompanyApp.Wpf.Helpers;

public sealed class EnumItem<T> where T : struct, Enum
{
    public EnumItem(T value)
    {
        Value = value;
        DisplayName = GetDescription(value);
    }

    public T Value { get; }

    public string DisplayName { get; }

    private static string GetDescription(T value)
    {
        var field = typeof(T).GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}

public static class EnumHelper
{
    public static IReadOnlyList<EnumItem<T>> GetItems<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().Select(v => new EnumItem<T>(v)).ToList();
    }
}
