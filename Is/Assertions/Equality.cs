using System.Collections;

namespace Is.Assertions;

public static class Equality
{
	/// <summary>
	/// Asserts that the actual object is equal to the expected value.
	/// (no array unwrapping, exact match for floating points)
	/// </summary>
	public static bool IsExactly<T>(this T actual, T expected) =>
		actual.IsExactlyEqualTo(expected).ThrowIf(actual, "is not", expected);

	/// <summary>
	/// Asserts that the actual object matches the expected value(s).
	/// (array unwrapping, approximately for floating points)
	/// </summary>
	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	/// <summary>
	/// Asserts that the actual value is not equal to the expected value.
	/// </summary>
	public static bool IsNot<T>(this T actual, T expected) =>
		(!actual.IsExactlyEqualTo(expected)).ThrowIf(actual, "is", expected);

	private static bool ShouldBe(this object actual, object[]? expected) =>
		expected?.Length switch
		{
			null => actual.IsEqualTo(null),
			1 when !actual.IsEnumerable() => actual.IsEqualTo(expected[0]),
			_ => actual.ToArray().Are(expected)
		};

	private static object[] Unwrap(this object[] array)
	{
		if (array.Length == 1 && array[0].IsEnumerable())
			return array[0].ToArray();

		return array;
	}

	private static bool Are(this object[] values, object[] expected)
	{
		if (values.Length == expected.Length)
			return Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]));

		throw new NotException(values, "are not", expected);
	}

	private static bool IsEqualTo<T>(this T? actual, T? expected)
	{
		if (actual.IsExactlyEqualTo(expected))
			return true;

		if (actual.IsCloseTo(expected))
			return true;

		throw new NotException(actual, "is not", expected);
	}

	private static bool IsExactlyEqualTo<T>(this T? actual, T? expected) =>
		EqualityComparer<T>.Default.Equals(actual, expected);

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
}