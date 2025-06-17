using System.Diagnostics;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections;

namespace Is;

[DebuggerStepThrough]
public class NotException : Exception
{
	public NotException(object? actual, string equality, object? expected)
		: base(actual.Actually(equality, expected).AddCodeLine())
	{ }

	public NotException(object? actual, string equality)
		: base(actual.Actually(equality).AddCodeLine())
	{ }

	public NotException(string message, List<string> text, int max = 100)
		: base($"{message}\n\n\t{string.Join("\n\t", text.Truncate(max))}\n".AddCodeLine())
	{ }
}

[DebuggerStepThrough]
internal static class MessageExtensions
{
	private static readonly bool ColorSupport = Console.IsOutputRedirected || !OperatingSystem.IsWindows();

	const int RIGHT = 32;
	const int WRONG = 91;

	internal static string Simply(this object? actual, string equality, object? expected) =>
		actual.Color(WRONG) + " " + equality + " " + expected.Color(RIGHT);

	internal static string Actually(this object? actual, string equality, object? expected) =>
		CreateMessage(actual.Format().Color(WRONG), equality, expected.Format().Color(RIGHT));

	internal static string Actually(this object? actual, string equality) =>
		CreateMessage(actual.Format().Color(WRONG), equality);

	internal static IEnumerable<string> Truncate(this List<string> text, int max) =>
		text.Count > max ? text.Take(max).Append("...") : text;

	// https://ss64.com/nt/syntax-ansi.html
	internal static string? Color<T>(this T text, int color) =>
		ColorSupport ? "\x1b[" + color + "m" + text + "\x1b[0m" : text?.ToString();

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

	private static string FormatType(this object? value) =>
		value is null or Type ? "" : $" ({value.GetType().Name})";

	private static string Join(this IEnumerable<string?> items, string start, string separator, string end) =>
		start + string.Join(separator, items) + end;

	private static string CreateMessage(params string?[] content) =>
		content.Join("\n\t", "\n\n\t", "\n");

	private static string Strip(this string text, int length = 50) =>
		text.Length <= length ? text : text[..length];

}

[DebuggerStepThrough]
file static class CallStackExtensions
{
	private static readonly ConcurrentDictionary<string, string[]> SourceCache = new();

	internal static string AddCodeLine(this string text) =>
		"\n" + text + "\n" + FindFrame()?.CodeLine() + "\n";

	private static StackFrame? FindFrame() =>
		new StackTrace(true).GetFrames().FirstOrDefault(f => f.IsForeignAssembly() && f.GetFileName() != null);

	private static bool IsForeignAssembly(this StackFrame frame) =>
		frame.GetMethod()?.DeclaringType?.Assembly != Assembly.GetExecutingAssembly();

	private static string CodeLine(this StackFrame frame) => "in " +
		frame.GetMethod()?.DeclaringType.Color(1) + frame.GetFileName()?.GetLine(frame.GetFileLineNumber());

	private static string GetLine(this string fileName, int lineNumber) => " in line " + lineNumber.Color(1) + ": " +
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim().Color(93);
}