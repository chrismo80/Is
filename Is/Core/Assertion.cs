using System.Diagnostics;
using System.Collections;

namespace Is;

[DebuggerStepThrough]
internal static class Assertion
{
	internal static bool Passed() => Passed(true);

	internal static T Passed<T>(T result)
	{
		AssertionContext.Current?.AddSuccess();

		return result;
	}

	internal static T Failed<T>(string message)
	{
		var ex = new NotException(message);

		if (Configuration.ThrowOnFailure && !AssertionContext.IsActive)
			throw ex;

		AssertionContext.Current?.AddFailure(ex);

		Configuration.Logger?.Invoke(ex.Message);

		return default;
	}

	internal static T Failed<T>(object? actual, string equality, object? expected) =>
		Failed<T>(actual.Actually(equality, expected));

	internal static T Failed<T>(object? actual, string equality) =>
		Failed<T>(actual.Actually(equality));

	internal static T Failed<T>(string message, List<string> text, int max = 100) =>
		Failed<T>($"{message}\n\n\t{string.Join("\n\t", text.Truncate(max))}\n");
}

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