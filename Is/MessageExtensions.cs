namespace Is;

using System.Collections;

public static class MessageExtensions
{
    internal static string Actually(this object? actual, string equality, object? expected) =>
        "\n" + actual.Format() + "\n" + equality + "\n" + expected.Format() + "\n";

    internal static string Format(this object? value) =>
        value.FormatValue() + value.FormatType();

    private static string FormatValue(this object? value) =>
        value switch
        {
            null => "<NULL>",
            string => $"\"{value}\"",
            IEnumerable list => $"[{list.FormatArray()}]",
            _ => $"{value}"
        };

    private static string FormatArray(this IEnumerable list) =>
        string.Join("|", list.Cast<object>().Select(x => x.FormatValue()));

    private static string FormatType(this object? value) =>
        value is null or Type ? "" : $" ({value.GetType()})";
}