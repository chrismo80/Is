namespace Is;

using System.Collections;

public static class MessageExtensions
{
    internal static string Actually(this object? actual, string equality, object? expected) =>
        CreateMessage(actual.Format(), equality, expected.Format());

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

    private static string CreateMessage(params string[] content) =>
        "\n" + string.Join("\n", content) + "\n";
}