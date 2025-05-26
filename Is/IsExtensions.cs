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
		try
		{
			action();
		}
		catch (Exception ex)
		{
			return ex.Is<T>();
		}

		throw new IsNotException($"\n{typeof(T)}\nactually was not thrown");
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
	public static bool Is(this object actual, params object[]? expected) => actual.ShouldBe(expected?.Unwrap());

	/// <summary>
	/// Asserts that the actual object is equal to the expected value (exact match).
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <param name="expected">The expected value.</param>
	/// <returns>True if values are equal.</returns>
	/// <exception cref="IsNotException">Thrown if values are not equal.</exception>
	public static bool IsExactly(this object actual, object expected) => actual.IsEqualTo(expected);

	/// <summary>
	/// Asserts that the actual object is <c>null</c>.
	/// </summary>
	/// <param name="actual">The actual value.</param>
	/// <returns>True if the object is null.</returns>
	/// <exception cref="IsNotException">Thrown if the object is not null.</exception>
	public static bool IsNull(this object actual) => actual.IsEqualTo(null);

	/// <summary>
	/// Asserts that a boolean value is <c>true</c>.
	/// </summary>
	/// <param name="actual">The actual boolean to check.</param>
	/// <returns>True if the value is true.</returns>
	/// <exception cref="IsNotException">Thrown if the value is false.</exception>
	public static bool IsTrue(this bool actual) => actual.IsEqualTo(true);

	/// <summary>
	/// Asserts that a boolean value is <c>false</c>.
	/// </summary>
	/// <param name="actual">The actual boolean to check.</param>
	/// <returns>True if the value is false.</returns>
	/// <exception cref="IsNotException">Thrown if the value is true.</exception>
	public static bool IsFalse(this bool actual) => actual.IsEqualTo(false);

	/// <summary>
	/// Asserts that the sequence is empty.
	/// </summary>
	/// <param name="actual">The enumerable to check.</param>
	/// <returns>True if the enumerable is empty.</returns>
	/// <exception cref="IsNotException">Thrown if the enumerable is not empty.</exception>
	public static bool IsEmpty(this IEnumerable actual)
	{
		if (!actual.Cast<object>().Any())
			return true;

		throw new IsNotException($"\n{actual.Format()}\nactually is not empty\n");
	}

	/// <summary>
	/// Asserts that the actual value is greater than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A comparable type.</typeparam>
	/// <param name="actual">The actual value.</param>
	/// <param name="other">The value to compare against.</param>
	/// <returns>True if the actual value is greater than <paramref name="other" />.</returns>
	/// <exception cref="IsNotException">Thrown if not greater.</exception>
	public static bool IsGreaterThan<T>(this T actual, T other) where T : IComparable<T>
	{
		if(actual.CompareTo(other) > 0)
			return true;

		throw new IsNotException(actual.Actually("is not greater than", other));
	}

	/// <summary>
	/// Asserts that the actual value is smaller than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A comparable type.</typeparam>
	/// <param name="actual">The actual value.</param>
	/// <param name="other">The value to compare against.</param>
	/// <returns>True if the actual value is smaller than <paramref name="other" />.</returns>
	/// <exception cref="IsNotException">Thrown if not smaller.</exception>
	public static bool IsSmallerThan<T>(this T actual, T other) where T : IComparable<T>
	{
		if(actual.CompareTo(other) < 0)
			return true;

		throw new IsNotException(actual.Actually("is not smaller than", other));
	}
}

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
		if(array.Length == 1 && array[0].IsEnumerable())
			return array[0].ToArray();

		return array;
	}

	private static bool IsEnumerable(this object value) => value is IEnumerable and not string;

	private static object[] ToArray(this object value) => Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static bool Are(this object[] values, object[] expected)
	{
		if (values.Length == expected.Length)
			return Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]));

		throw new IsNotException(values.Actually("are not", expected));
	}

	internal static bool IsEqualTo<T>(this T? actual, T? expected)
	{
		if(EqualityComparer<T>.Default.Equals(actual, expected))
			return true;

		if(actual.IsCloseTo(expected))
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

	private static bool IsInTolerance<T>(this T actual, T expected, T epsilon) where T : IFloatingPoint<T>
	{
		if(T.Abs(actual - expected) <= epsilon * T.Max(T.One, T.Abs(expected)))
			return true;

		throw new IsNotException(actual.Actually("is not close enough to", expected));
	}
}

public class IsNotException(string message) : Exception(message.PrependCodeLine());

file static class MessageExtensions
{
	internal static string Actually(this object? actual, string equality, object? expected) =>
		CreateMessage(actual.Format(), "actually " + equality, expected.Format());

	internal static string Format(this object? value) =>
		value.FormatValue() + value.FormatType();

	private static string FormatValue(this object? value) =>
		value switch
		{
			null => "<NULL>",
			string => $"\"{value}\"",
			IEnumerable list => list.Cast<object>().Select(x => x.FormatValue()).Join("[", "|", "]"),
			_ => $"{value}"
		};

	private static string FormatType(this object? value) =>
		value is null or Type ? "" : $" ({value.GetType()})";

	private static string Join(this IEnumerable<string> items, string start, string separator, string end) =>
		start + string.Join(separator, items) + end;

	private static string CreateMessage(params string[] content) =>
		content.Join("\n", "\n\n", "\n");

	internal static string PrependCodeLine(this string text) =>
#if DEBUG
		"\n\n" + FindFrame()?.CodeLine() +
#endif
		"\n\n" + text;

	private static StackFrame? FindFrame() =>
		new StackTrace(true).GetFrames().FirstOrDefault(f => f.IsOtherNamespace() && f.GetFileName() != null);

	private static bool IsOtherNamespace(this StackFrame frame) =>
		frame.GetMethod()?.DeclaringType?.Namespace != typeof(IsNotException).Namespace;

	private static string? CodeLine(this StackFrame frame) =>
		frame.GetFileName()?.GetLine(frame.GetFileLineNumber());

	private static string GetLine(this string? fileName, int lineNumber) => fileName == null ? ""
		: "Line " + lineNumber + ": " + File.ReadLines(fileName).Skip(lineNumber - 1).FirstOrDefault()?.Trim();
}