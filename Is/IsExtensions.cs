namespace Is;

using System.Numerics;
using System.Collections;

public static class IsExtensions
{
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

		throw new IsNotException($"No {typeof(T)} was thrown");
	}

	public static T Is<T>(this object actual) =>
		actual is T cast ? cast : throw new IsNotException(actual.Actually("is no", typeof(T)));

	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	public static bool IsExactly(this object actual, object expected) =>
		actual.IsEqualTo(expected);

	public static bool IsNull(this object actual) =>
		actual.IsEqualTo(null);

	public static bool IsTrue(this bool actual) =>
		actual.IsEqualTo(true);

	public static bool IsFalse(this bool actual) =>
		actual.IsEqualTo(false);

	public static bool IsEmpty(this IEnumerable actual) =>
		!actual.Cast<object>().Any() ? true
			: throw new IsNotException($"{actual.Format()} actually is not empty");

	public static bool IsGreaterThan<T>(this T actual, T other) where T : IComparable<T> =>
		actual.CompareTo(other) > 0 ? true
			: throw new IsNotException(actual.Actually("is not greater than", other));

	public static bool IsSmallerThan<T>(this T actual, T other) where T : IComparable<T> =>
		actual.CompareTo(other) < 0 ? true
			: throw new IsNotException(actual.Actually("is not smaller than", other));

	private static bool ShouldBe(this object actual, object[]? expected) =>
		expected?.Length switch
		{
			null => actual.IsEqualTo(null),
			1 => actual.IsEqualTo(expected[0]),
			_ => actual.ToArray().Are(expected)
		};

	private static object[] Unwrap(this object[] array) =>
		array is [IEnumerable first and not string] ? first.ToArray() : array;

	private static object[] ToArray(this object value) =>
		Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static bool Are(this object[] values, object[] expected) =>
		values.Length == expected.Length ? Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]))
			: throw new IsNotException(values.Actually("are not", expected));

	private static bool IsEqualTo<T>(this T? actual, T? expected) =>
		EqualityComparer<T>.Default.Equals(actual, expected) || actual.IsCloseTo(expected) ? true
			: throw new IsNotException(actual.Actually("is not", expected));

	private static bool IsCloseTo<T>(this T? actual, T? expected) =>
		(actual, expected) switch
		{
			(float a, float e) => a.IsInTolerance(e, 1e-6f),
			(double a, double e) => a.IsInTolerance(e, 1e-6),
			_ => false
		};

	private static bool IsInTolerance<T>(this T actual, T expected, T tolerance) where T : IFloatingPoint<T> =>
		T.Abs(actual - expected) <= tolerance * T.Max(T.One, T.Abs(expected)) ? true
			: throw new IsNotException(actual.Actually("is not close to", expected));
}