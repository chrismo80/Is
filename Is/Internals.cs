using System.Collections;
using System.Numerics;

using Is.Assertions;

namespace Is;

internal static class Internals
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