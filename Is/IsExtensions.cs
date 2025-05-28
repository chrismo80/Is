using System.Text.RegularExpressions;
using System.Collections;
using System.Numerics;
using System.Diagnostics;

namespace Is;

public static class IsExtensions
{
	/// <summary>
	/// Asserts that the given <paramref name="action" /> throws an exception of type <typeparamref name="T" />.
	/// </summary>
	/// <typeparam name="T">The expected exception type.</typeparam>
	/// <param name="action">The action expected to throw an exception.</param>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	/// <exception cref="IsNotException">Thrown if no exception is thrown or the type does not match.</exception>
	public static T IsThrowing<T>(this Action action) where T : Exception
	{
		try { action(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new IsNotException($"\n{typeof(T).Color(1)}\nactually was not thrown");
	}

	/// <summary>
	/// Asserts that the given async <paramref name="action" /> throws an exception of type <typeparamref name="T" />.
	/// </summary>
	/// <typeparam name="T">The expected exception type.</typeparam>
	/// <param name="action">The async action expected to throw an exception.</param>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	/// <exception cref="IsNotException">Thrown if no exception is thrown or the type does not match.</exception>
	public static T IsThrowing<T>(this Func<Task> action) where T : Exception
	{
		try { action().GetAwaiter().GetResult(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new IsNotException($"\n{typeof(T).Color(1)}\nactually was not thrown");
	}

	/// <summary>
	/// Asserts that the actual object is of type <typeparamref name="T" /> and returns its cast.
	/// </summary>
	/// <typeparam name="T">The expected type.</typeparam>
	/// <param name="actual">The actual object to check.</param>
	/// <returns>The object cast to type <typeparamref name="T" />.</returns>
	/// <exception cref="IsNotException">Thrown if the actual object is not of that type.</exception>
	public static T Is<T>(this object actual)
	{
		if (actual is T cast)
			return cast;

		throw new IsNotException(actual.Actually("is no", typeof(T)));
	}

	/// <summary>
	/// Asserts that the actual object matches the expected value(s).
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value(s) to match against.</param>
	/// <returns>True if matching.</returns>
	/// <exception cref="IsNotException">Thrown if not matching.</exception>
	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	/// <summary>
	/// Asserts that the actual object is equal to the expected value (exact match).
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <returns>True if values are equal.</returns>
	/// <exception cref="IsNotException">Thrown if values are not equal.</exception>
	public static bool IsExactly<T>(this T actual, T expected) =>
		actual.IsExactlyEqualTo(expected).ThrowIf(actual, "is not exactly", expected);

	/// <summary>
	/// Asserts that the sequence is empty.
	/// </summary>
	/// <param name="actual">The enumerable to check.</param>
	/// <returns>True if the enumerable is empty.</returns>
	/// <exception cref="IsNotException">Thrown if the enumerable is not empty.</exception>
	public static bool IsEmpty<T>(this IEnumerable<T> actual)
	{
		if (!actual.Any())
			return true;

		throw new IsNotException($"\n{actual.Format().Color(1)}\nactually is not empty");
	}

	/// <summary>
	/// Asserts that the actual value is greater than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A comparable type.</typeparam>
	/// <param name="actual">The actual value.</param>
	/// <param name="other">The value to compare against.</param>
	/// <returns>True if the actual value is greater than <paramref name="other" />.</returns>
	/// <exception cref="IsNotException">Thrown if not greater.</exception>
	public static bool IsGreaterThan<T>(this T actual, T other) where T : IComparable<T> =>
		(actual.CompareTo(other) > 0).ThrowIf(actual, "is not greater than", other);

	/// <summary>
	/// Asserts that the actual value is smaller than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A comparable type.</typeparam>
	/// <param name="actual">The actual value.</param>
	/// <param name="other">The value to compare against.</param>
	/// <returns>True if the actual value is smaller than <paramref name="other" />.</returns>
	/// <exception cref="IsNotException">Thrown if not smaller.</exception>
	public static bool IsSmallerThan<T>(this T actual, T other) where T : IComparable<T> =>
		(actual.CompareTo(other) < 0).ThrowIf(actual, "is not smaller than", other);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence contains all the specified <paramref name="expected"/> elements.
	/// Throws an <see cref="IsNotException"/> if any expected element is missing.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="actual">The collection to check.</param>
	/// <param name="expected">The elements that must be present in <paramref name="actual"/>.</param>
	/// <returns>True if all expected elements are found in the collection.</returns>
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected) =>
		expected.All(actual.Contains).ThrowIf(actual, "is not containing", expected);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string contains the specified <paramref name="expected"/> substring.
	/// Throws an <see cref="IsNotException"/> if <paramref name="expected"/> is not found.
	/// </summary>
	/// <param name="actual">The string to search in.</param>
	/// <param name="expected">The substring expected to be found.</param>
	/// <returns>True if <paramref name="expected"/> is found in <paramref name="actual"/>.</returns>
	public static bool IsContaining(this string actual, string expected) =>
		actual.Contains(expected).ThrowIf(actual, "is not containing", expected);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string matches the specified <paramref name="pattern"/> regular expression.
	/// Throws an <see cref="IsNotException"/> if the string does not match the pattern.
	/// </summary>
	/// <param name="actual">The string to check.</param>
	/// <param name="pattern">The regular expression pattern to match.</param>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	public static GroupCollection IsMatching(this string actual, string pattern)
	{
		if (Regex.Match(actual, pattern) is { Success: true } match)
			return match.Groups;

		throw new IsNotException(actual.Actually("does not match", pattern));
	}
}

public static class OptionalExtensions
{
	/// <summary>
	/// Asserts that the given synchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/>
	/// and that the exception message contains the specified <paramref name="message"/> substring.
	/// Throws an <see cref="IsNotException"/> if the assertion fails.
	/// </summary>
	/// <typeparam name="T">The expected exception type.</typeparam>
	/// <param name="action">The action expected to throw the exception.</param>
	/// <param name="message">A substring expected to be found in the exception message.</param>
	/// <returns>True if the exception is thrown and its message contains <paramref name="message"/>.</returns>
	public static bool IsThrowing<T>(this Action action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);

	/// <summary>
	/// Asserts that the given asynchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/>
	/// and that the exception message contains the specified <paramref name="message"/> substring.
	/// Throws an <see cref="IsNotException"/> if the assertion fails.
	/// </summary>
	/// <typeparam name="T">The expected exception type.</typeparam>
	/// <param name="action">The asynchronous function expected to throw the exception.</param>
	/// <param name="message">A substring expected to be found in the exception message.</param>
	/// <returns>True if the exception is thrown and its message contains <paramref name="message"/>.</returns>
	public static bool IsThrowing<T>(this Func<Task> action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);

	/// <summary>
	/// Asserts that all elements in the <paramref name="actual"/> collection are present in the specified <paramref name="expected"/> collection.
	/// Throws an <see cref="IsNotException"/> if any element is not found.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	/// <param name="actual">The collection to check.</param>
	/// <param name="expected">The collection to search in.</param>
	/// <returns>True if all elements in the actual collection are found in the expected collection.</returns>
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected) =>
		expected.IsContaining(actual.ToArray());

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is strictly between <paramref name="min"/> and <paramref name="max"/>.
	/// Throws an <see cref="IsNotException"/> if the condition is not met.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable{T}"/>.</typeparam>
	/// <param name="actual">The value to check.</param>
	/// <param name="min">The lower exclusive bound.</param>
	/// <param name="max">The upper exclusive bound.</param>
	/// <returns>True if <paramref name="actual"/> is greater than <paramref name="min"/> and less than <paramref name="max"/>.</returns>
	public static bool IsBetween<T>(this T actual, T min, T max) where T : IComparable<T> =>
		actual.IsGreaterThan(min) && actual.IsSmallerThan(max);

	/// <summary>
	/// Asserts that the actual object is <c>null</c>.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <returns>True if the object is null.</returns>
	/// <exception cref="IsNotException">Thrown if the object is not null.</exception>
	public static bool IsNull(this object actual) =>
		actual.IsExactly(null);

	/// <summary>
	/// Asserts that a boolean value is <c>true</c>.
	/// </summary>
	/// <param name="actual">The actual boolean to check.</param>
	/// <returns>True if the value is true.</returns>
	/// <exception cref="IsNotException">Thrown if the value is false.</exception>
	public static bool IsTrue(this bool actual) =>
		actual.IsExactly(true);

	/// <summary>
	/// Asserts that a boolean value is <c>false</c>.
	/// </summary>
	/// <param name="actual">The actual boolean to check.</param>
	/// <returns>True if the value is false.</returns>
	/// <exception cref="IsNotException">Thrown if the value is true.</exception>
	public static bool IsFalse(this bool actual) =>
		actual.IsExactly(false);

	/// <summary>
	/// Determines whether the specified <paramref name="actual"/> value is approximately equal to <paramref name="expected"/>
	/// within a specified <paramref name="epsilon"/> tolerance.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="epsilon">The maximum allowed difference. Defaults to 1e-6.</param>
	/// <returns>True if the absolute difference is less than or equal to epsilon; otherwise, false.</returns>
	public static bool IsApproximately(this double actual, double expected, double epsilon = 1e-6) =>
		actual.IsInTolerance(expected, epsilon);

	/// <summary>
	/// Determines whether the specified <paramref name="actual"/> value is approximately equal to <paramref name="expected"/>
	/// within a specified <paramref name="epsilon"/> tolerance.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <param name="epsilon">The maximum allowed difference. Defaults to 1e-6.</param>
	/// <returns>True if the absolute difference is less than or equal to epsilon; otherwise, false.</returns>
	public static bool IsApproximately(this float actual, float expected, float epsilon = 1e-6f) =>
		actual.IsInTolerance(expected, epsilon);
}

public class IsNotException(string message) : Exception(message.AddCodeLine())
{ }

file static class InternalExtensions
{
	internal static bool ShouldBe(this object actual, object[]? expected) =>
		expected?.Length switch
		{
			null => actual.IsEqualTo(null),
			1 when !actual.IsEnumerable() => actual.IsEqualTo(expected[0]),
			_ => actual.ToArray().Are(expected)
		};

	internal static object[] Unwrap(this object[] array)
	{
		if (array.Length == 1 && array[0].IsEnumerable())
			return array[0].ToArray();

		return array;
	}

	internal static bool IsExactlyEqualTo<T>(this T? actual, T? expected) =>
		EqualityComparer<T>.Default.Equals(actual, expected);

	internal static bool ThrowIf(this bool condition, object? actual, string text, object? expected)
	{
		if (condition)
			return true;

		throw new IsNotException(actual.Actually(text, expected));
	}

	internal static bool IsInTolerance<T>(this T actual, T expected, T epsilon) where T : IFloatingPoint<T>
	{
		if (T.Abs(actual - expected) <= epsilon * T.Max(T.One, T.Abs(expected)))
			return true;

		throw new IsNotException(actual.Actually("is not close enough to", expected));
	}

	private static bool IsEqualTo<T>(this T? actual, T? expected)
	{
		if (actual.IsExactlyEqualTo(expected))
			return true;

		if (actual.IsCloseTo(expected))
			return true;

		throw new IsNotException(actual.Actually("is not", expected));
	}

	private static bool IsCloseTo<T>(this T? actual, T? expected) =>
		(actual, expected) switch
		{
			(float a, float e) => a.IsInTolerance(e, 1e-6f),
			(double a, double e) => a.IsInTolerance(e, 1e-6),
			_ => false
		};

	private static bool IsEnumerable(this object value) => value is IEnumerable and not string;

	private static object[] ToArray(this object value) => Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static bool Are(this object[] values, object[] expected)
	{
		if (values.Length == expected.Length)
			return Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]));

		throw new IsNotException(values.Actually("are not", expected));
	}
}

file static class MessageExtensions
{
	private static readonly bool ColorSupport = Console.IsOutputRedirected || !OperatingSystem.IsWindows();

	internal static string Actually(this object? actual, string equality, object? expected) =>
		CreateMessage(actual.Format().Color(1), "actually " + equality, expected.Format().Color(2));

	internal static string Format(this object? value) =>
		value.FormatValue() + value.FormatType();

	internal static string? Color<T>(this T text, int color) =>
		ColorSupport ? "\x1b[3" + color + "m" + text + "\x1b[0m" : text?.ToString();

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

	private static string Join(this IEnumerable<string> items, string start, string separator, string end) =>
		start + string.Join(separator, items) + end;

	private static string CreateMessage(params string[] content) =>
		content.Join("\n\t", "\n\n\t", "\n");
}

file static class CallStackExtensions
{
	internal static string AddCodeLine(this string text) =>
		"\n\n" + text + "\n\n" + FindFrame()?.CodeLine().Color(3)?.AddLines();

	private static StackFrame? FindFrame() =>
		new StackTrace(true).GetFrames().FirstOrDefault(f => f.IsOtherNamespace() && f.GetFileName() != null);

	private static bool IsOtherNamespace(this StackFrame frame) =>
		frame.GetMethod()?.DeclaringType?.Namespace != typeof(IsNotException).Namespace;

	private static string CodeLine(this StackFrame frame) =>
		"in " + frame.GetMethod()?.DeclaringType + frame.GetFileName()?.GetLine(frame.GetFileLineNumber());

	private static string GetLine(this string? fileName, int lineNumber) => fileName == null ? ""
		: " in line " + lineNumber + ": " + File.ReadLines(fileName).Skip(lineNumber - 1).FirstOrDefault()?.Trim();

	private static string AddLines(this string line) =>
		line + "\n" + string.Concat(Enumerable.Range(0, line.Length).Select(_ => "‾"));
}