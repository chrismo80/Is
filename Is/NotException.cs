using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Is;

public class NotException : Exception
{
	public NotException(object? actual, string equality, object? expected)
		: base(actual.Actually(equality, expected).AddCodeLine())
	{ }

	public NotException(object? actual, string equality)
		: base(actual.Actually(equality).AddCodeLine())
	{ }
}

file static class MessageExtensions
{
	private static readonly bool ColorSupport = Console.IsOutputRedirected || !OperatingSystem.IsWindows();

	internal static string Actually(this object? actual, string equality, object? expected) =>
		CreateMessage(actual.Format().Color(31), "actually " + equality, expected.Format().Color(32));

	internal static string Actually(this object? actual, string equality) =>
		CreateMessage(actual.Format().Color(31), "actually " + equality);

	private static string Format(this object? value) =>
		value.FormatValue() + value.FormatType();

	internal static string? Color<T>(this T text, int color) =>
		ColorSupport ? "\x1b[" + color + "m" + text + "\x1b[0m" : text?.ToString();

	private static string FormatValue(this object? value) =>
		value switch
		{
			null => "<NULL>",
			string => $"\"{value}\"",
			IEnumerable list => list.Cast<object>().Select(x => x.FormatValue()).Join("[", "|", "]"),
			Exception ex => ex.Message,
			_ => $"{value}"
		};

	private static string FormatType(this object? value) =>
		value is null or Type ? "" : $" ({value.GetType()})";

	private static string Join(this IEnumerable<string?> items, string start, string separator, string end) =>
		start + string.Join(separator, items) + end;

	private static string CreateMessage(params string?[] content) =>
		content.Join("\n\t", "\n\n\t", "\n");
}

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
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim().Color(33);
}