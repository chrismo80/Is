using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections;
using System.Text.Json;

namespace Is;

[DebuggerStepThrough]
public static class Equality
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> object is equal to the <paramref name="expected"/> value.
	/// (no array unwrapping, exact match for floating points)
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsExactly<T>(this T actual, T expected)
	{
		if (actual.IsExactlyEqualTo(expected))
			return true;

		throw new NotException(actual, "is not", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> object matches the <paramref name="expected"/> value(s).
	/// (array unwrapping, approximately for floating points)
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is not equal to the <paramref name="expected"/> value.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this T actual, T expected)
	{
		if (!actual.IsExactlyEqualTo(expected))
			return true;

		throw new NotException(actual, "is", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> object is the same instance as the <paramref name="expected"/> object.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsSameAs<T>(this T actual, T expected) where T : class?
	{
		if (ReferenceEquals(actual, expected))
			return true;

		throw new NotException(actual, "is not the same instance as", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is the default value of its type.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsDefault<T>(this T actual) =>
		actual.IsExactly(default);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> object satisfies the specified <paramref name="predicate"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsSatisfying<T>(this T actual, Func<T, bool> predicate)
	{
		if (predicate(actual))
			return true;

		throw new NotException(actual, "is not satisfying the predicate");
	}

	/// <summary>
	/// Asserts that the given <paramref name="actual" /> object matches the
	/// <paramref name="expected" /> by comparing their serialized JSON strings for equality.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsMatchingSnapshot<T>(this T actual, T expected, JsonSerializerOptions? options = null)
	{
		var (actualJson, expectedJson) = (actual.ToJson(options), expected.ToJson(options));

		if (actualJson.IsExactlyEqualTo(expectedJson))
			return true;

		throw new  NotException("object is not matching", actualJson.DifferencesTo(expectedJson), 10);
	}

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
			(double a, double e) => a.IsApproximately(e),
			(float a, float e) => a.IsApproximately(e),
			(decimal a, decimal e) => a.IsApproximately(e),
			_ => false
		};

	private static bool IsEnumerable(this object value) =>
		value is IEnumerable and not string;

	private static object[] ToArray(this object value) =>
		Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());
}