using System.Text.RegularExpressions;
using System.Collections;
using System.Numerics;

namespace Is;

public static class ExceptionAssertions
{
	/// <summary>Asserts that the given <paramref name="action" /> throws an exception of type <typeparamref name="T" />.</summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	public static T IsThrowing<T>(this Action action) where T : Exception
	{
		try { action(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new NotException(typeof(T), "was not thrown");
	}

	/// <summary>Asserts that the given async <paramref name="action" /> throws an exception of type <typeparamref name="T" />.</summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	public static T IsThrowing<T>(this Func<Task> action) where T : Exception
	{
		try { action().GetAwaiter().GetResult(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new NotException(typeof(T), "was not thrown");
	}

	/// <summary>Asserts that the given synchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/> and that the exception message contains the specified <paramref name="message"/> substring.</summary>
	public static bool IsThrowing<T>(this Action action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);

	/// <summary>Asserts that the given asynchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/> and that the exception message contains the specified <paramref name="message"/> substring.</summary>
	public static bool IsThrowing<T>(this Func<Task> action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);
}

public static class TypeAssertions
{
	/// <summary>Asserts that the actual object is of type <typeparamref name="T" />.</summary>
	/// <returns>The cast object to the type <typeparamref name="T" />.</returns>
	public static T Is<T>(this object actual)
	{
		if (actual is T cast)
			return cast;

		throw new NotException(actual, "is no", typeof(T));
	}

	/// <summary>Asserts that the actual object is not of type <typeparamref name="T"/>.</summary>
	public static bool IsNot<T>(this object actual) =>
		(actual is not T).ThrowIf(actual, "is", typeof(T));
}

public static class EqualityAssertions
{
	/// <summary>Asserts that the actual object is equal to the expected value. (no array unwrapping, exact match for floating points)</summary>
	public static bool IsExactly<T>(this T actual, T expected) =>
		actual.IsExactlyEqualTo(expected).ThrowIf(actual, "is not", expected);

	/// <summary>Asserts that the actual object matches the expected value(s). (array unwrapping, approximately for floating points)</summary>
	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	/// <summary>Asserts that the actual value is not equal to the expected value.</summary>
	public static bool IsNot<T>(this T actual, T expected) =>
		(!actual.IsExactlyEqualTo(expected)).ThrowIf(actual, "is", expected);
}

public static class CollectionAssertions
{
	/// <summary>Asserts that the sequence is empty.</summary>
	public static bool IsEmpty<T>(this IEnumerable<T> actual) =>
		(!actual.Any()).ThrowIf(actual, "is not empty");

	/// <summary>Asserts that the <paramref name="actual"/> sequence contains all the specified <paramref name="expected"/> elements.</summary>
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull =>
		actual.CountDiff(expected).All(c => c >= 0).ThrowIf(actual, "is not containing", expected);

	/// <summary>Asserts that all elements in the <paramref name="actual"/> collection are present in the <paramref name="expected"/> collection.</summary>
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull =>
		actual.CountDiff(expected).All(c => c <= 0).ThrowIf(actual, "is not in", expected);

	/// <summary>Asserts that the <paramref name="actual"/> sequence matches the <paramref name="expected"/> sequence ignoring item order.</summary>
	public static bool IsEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected) where T : notnull =>
		actual.CountDiff(expected).All(c => c == 0).ThrowIf(actual, "is not unordered", expected);
}

public static class ComparisonAssertions
{
	/// <summary>Asserts that the <paramref name="actual"/> floating point is approximately equal to <paramref name="expected"/>.</summary>
	/// <typeparam name="T">A type that implements <see cref="IFloatingPoint{TSelf}"/>.</typeparam>
	public static bool IsApproximately<T>(this T actual, T expected, T epsilon) where T : IFloatingPoint<T> =>
		actual.IsNear(expected, epsilon).ThrowIf(actual, "is not close enough to", expected);

	/// <summary>default epsilon is 1e-6.</summary>
	public static bool IsApproximately<T>(this T actual, T expected)  where T : IFloatingPoint<T> =>
		actual.IsApproximately(expected, T.CreateChecked(1e-6));

	/// <summary>Asserts that the actual value is greater than the given <paramref name="other" /> value.</summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsGreaterThan<T>(this T actual, T other) where T : IComparable<T> =>
		(actual.CompareTo(other) > 0).ThrowIf(actual, "is not greater than", other);

	/// <summary>Asserts that the actual value is smaller than the given <paramref name="other" /> value.</summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsSmallerThan<T>(this T actual, T other) where T : IComparable<T> =>
		(actual.CompareTo(other) < 0).ThrowIf(actual, "is not smaller than", other);

	/// <summary>Asserts that the <paramref name="actual"/> value is between <paramref name="min"/> and <paramref name="max"/> exclusive bounds.</summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsBetween<T>(this T actual, T min, T max) where T : IComparable<T> =>
		actual.IsGreaterThan(min) && actual.IsSmallerThan(max);

	/// <summary>Asserts that the actual value is not between the specified <paramref name="min"/> and <paramref name="max"/> exclusive bounds.</summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsNotBetween<T>(this T actual, T min, T max) where T : IComparable<T> =>
		(actual.CompareTo(max) > 0) || (actual.CompareTo(min) < 0).ThrowIf(actual, $"is between {min} and {max}");
}

public static class StringAssertions
{
	/// <summary>Asserts that the <paramref name="actual"/> string contains the specified <paramref name="expected"/> substring.</summary>
	public static bool IsContaining(this string actual, string expected) =>
		actual.Contains(expected).ThrowIf(actual, "is not containing", expected);

	/// <summary>Asserts that the <paramref name="actual"/> string matches the specified <paramref name="pattern"/> regular expression.</summary>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	public static GroupCollection IsMatching(this string actual, string pattern)
	{
		if (Regex.Match(actual, pattern) is { Success: true } match)
			return match.Groups;

		throw new NotException(actual, "does not match", pattern);
	}

	/// <summary>Asserts that the <paramref name="actual"/> string does not match the specified <paramref name="pattern"/> regular expression.</summary>
	public static bool IsNotMatching(this string actual, string pattern) =>
		(!Regex.Match(actual, pattern).Success).ThrowIf(actual, "is matching", pattern);
}

public static class BooleanAssertions
{
	/// <summary>Asserts that a boolean value is <c>true</c>.</summary>
	public static bool IsTrue(this bool actual) =>
		actual.IsExactly(true);

	/// <summary>Asserts that a boolean value is <c>false</c>.</summary>
	public static bool IsFalse(this bool actual) =>
		actual.IsExactly(false);
}

public static class NullAssertions
{
	/// <summary>Asserts that an object is <c>null</c>.</summary>
	public static bool IsNull(this object actual) =>
		actual.IsExactly(null);

	/// <summary>Asserts that the object is not <c>null</c>.</summary>
	public static bool IsNotNull(this object actual) =>
		(!actual.IsExactlyEqualTo(null)).ThrowIf(actual, "is null");
}

file static class Internals
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

		throw new NotException(actual, text, expected);
	}

	internal static bool ThrowIf(this bool condition, object? actual, string text)
	{
		if (condition)
			return true;

		throw new NotException(actual, text);
	}

	internal static bool IsNear<T>(this T actual, T expected, T epsilon) where T : IFloatingPoint<T> =>
		T.Abs(actual - expected) <= epsilon * T.Max(T.One, T.Abs(expected));

	internal static List<int> CountDiff<T>(this IEnumerable<T> left, IEnumerable<T> right) where T : notnull =>
		new Dictionary<T, int>().CountItems(left, 1).CountItems(right, -1).Values.ToList();

	private static Dictionary<T, int> CountItems<T>(this Dictionary<T, int> dict, IEnumerable<T> source, int increment) where T : notnull
	{
		foreach (var item in source)
			dict[item] = dict.GetValueOrDefault(item) + increment;

		return dict;
	}

	private static bool IsEqualTo<T>(this T? actual, T? expected)
	{
		if (actual.IsExactlyEqualTo(expected))
			return true;

		if (actual.IsCloseTo(expected))
			return true;

		throw new NotException(actual, "is not", expected);
	}

	private static bool IsCloseTo<T>(this T? actual, T? expected) =>
		(actual, expected) switch
		{
			(float a, float e) => a.IsApproximately(e),
			(double a, double e) => a.IsApproximately(e),
			_ => false
		};

	private static bool IsEnumerable(this object value) =>
		value is IEnumerable and not string;

	private static object[] ToArray(this object value) =>
		Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static bool Are(this object[] values, object[] expected)
	{
		if (values.Length == expected.Length)
			return Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]));

		throw new NotException(values, "are not", expected);
	}
}