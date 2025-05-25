using System.Collections;
using System.Diagnostics;

namespace Is;

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

    internal static string PrependCodeLine(this string text) =>
        FindFrame()?.CodeLine() + Environment.NewLine + text;

    private static StackFrame? FindFrame() => new StackTrace(true).GetFrames()
        .FirstOrDefault(f => f.GetMethod()?.DeclaringType?.Namespace != "Is" && f.GetFileName() != null);

    private static string CodeLine(this StackFrame frame) =>
        frame.GetFileName().GetLine(frame.GetFileLineNumber());

    private static string GetLine(this string? fileName, int lineNumber) => fileName == null ? ""
        : "Line " + lineNumber + ": " + File.ReadAllLines(fileName)[lineNumber - 1].Trim();
}