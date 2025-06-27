using System.Collections;
using System.Diagnostics;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class MessageExtensions
{
	private static readonly bool ColorSupport = Console.IsOutputRedirected || !OperatingSystem.IsWindows();

	const int RIGHT = 32;
	const int WRONG = 91;

	internal static string Simply(this object? actual, string equality, object? expected) =>
		actual?.GetType() == expected?.GetType() ?
			actual.FormatValue().Color(WRONG) + " " + equality + " " + expected.FormatValue().Color(RIGHT) :
			actual.Format().Color(WRONG) + " " + equality + " " + expected.Format().Color(RIGHT);

	internal static string Actually(this object? actual, string equality, object? expected) =>
		actual?.GetType() == expected?.GetType() ?
			CreateMessage(actual.FormatValue().Color(WRONG), equality, expected.FormatValue().Color(RIGHT)) :
			CreateMessage(actual.Format().Color(WRONG), equality, expected.Format().Color(RIGHT));

	internal static string Actually(this object? actual, string equality) =>
		CreateMessage(actual.Format().Color(WRONG), equality);

	internal static IEnumerable<string> Truncate(this List<string> text, int max) =>
		text.Count > max ? text.Take(max).Append("...") : text;

	// https://ss64.com/nt/syntax-ansi.html
	internal static string? Color<T>(this T text, int color) =>
		Configuration.Active.ColorizeMessages && ColorSupport ? "\x1b[" + color + "m" + text + "\x1b[0m" : text?.ToString();

	internal static string FormatValue(this object? value) =>
		value switch
		{
			null => "<NULL>",
			string => $"\"{value}\"",
			IEnumerable list => list.Cast<object>().Select(x => x.FormatValue()).Join("[", "|", "]").Strip(),
			Exception ex => ex.Message,
			_ => $"{value}"
		};

	internal static string Format(this object? value) =>
		value.FormatValue() + value.FormatType();

	internal static string RemoveLineBreaks(this string text) =>
		text.Replace("\n\n\t", " ").Replace("\n\n", " ");

	private static string FormatType(this object? value) =>
		value is null or Type ? "" : $" ({value.GetType().Name})";

	private static string Join(this IEnumerable<string?> items, string start, string separator, string end) =>
		start + string.Join(separator, items) + end;

	private static string CreateMessage(params string?[] content) =>
		content.Join("\n\t", "\n\n\t", "\n");

	private static string Strip(this string text, int length = 50) =>
		text.Length <= length ? text : text[..length];
}