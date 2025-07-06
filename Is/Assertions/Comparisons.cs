using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Numerics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Comparisons
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> floating point
	/// is approximately equal to <paramref name="expected"/> considering an <paramref name="precision"/>.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IFloatingPoint{TSelf}"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsApproximately<T>(this T actual, T expected, T precision) where T : IFloatingPoint<T> => Check
		.That(T.Abs(actual - expected) <= precision * T.Max(T.One, T.Abs(expected)))
		.Unless(actual, "is not approximately", expected);

	/// <summary>
	/// Uses default precision from configuration
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsApproximately<T>(this T actual, T expected) where T : IFloatingPoint<T> =>
		actual.IsApproximately(expected, T.CreateChecked(Configuration.Active.FloatingPointComparisonPrecision));

	/// <summary>
	/// Asserts that the <paramref name="actual"/> numeric value is positive (greater than zero).
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsPositive<T>(this T actual) where T : INumber<T> =>
		actual.IsGreaterThan(T.Zero);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> numeric value is negative (less than zero).
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNegative<T>(this T actual) where T : INumber<T> =>
		actual.IsSmallerThan(T.Zero);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is greater
	/// than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsGreaterThan<T>(this T actual, T other) where T : IComparable<T> => Check
		.That(actual.CompareTo(other) > 0)
		.Unless(actual, "is not greater than", other);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is smaller
	/// than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsSmallerThan<T>(this T actual, T other) where T : IComparable<T> => Check
		.That(actual.CompareTo(other) < 0)
		.Unless(actual, "is not smaller than", other);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is greater or equal
	/// the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsAtLeast<T>(this T actual, T other) where T : IComparable<T> => Check
		.That(actual.CompareTo(other) >= 0)
		.Unless(actual, "is smaller than", other);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is smaller or equal
	/// the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsAtMost<T>(this T actual, T other) where T : IComparable<T> => Check
		.That(actual.CompareTo(other) <= 0)
		.Unless(actual, "is greater than", other);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value
	/// is between <paramref name="min"/> and <paramref name="max"/> exclusive bounds.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsBetween<T>(this T actual, T min, T max) where T : IComparable<T> =>
		actual.IsGreaterThan(min) && actual.IsSmallerThan(max);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value
	/// is between <paramref name="min"/> and <paramref name="max"/> inclusive bounds.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsInRange<T>(this T actual, T min, T max) where T : IComparable<T> =>
		actual.IsAtLeast(min) && actual.IsAtMost(max);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value is not between
	/// the specified <paramref name="min"/> and <paramref name="max"/> exclusive bounds.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotBetween<T>(this T actual, T min, T max) where T : IComparable<T> => Check
		.That(max.IsAtLeast(min) && (actual.CompareTo(min) <= 0 || actual.CompareTo(max) >= 0))
		.Unless(actual, $"is between {min} and {max}");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value
	/// is smaller than <paramref name="min"/> or greater than <paramref name="max"/>.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsOutOfRange<T>(this T actual, T min, T max) where T : IComparable<T> => Check
		.That(max.IsAtLeast(min) && (actual.CompareTo(min) < 0 || actual.CompareTo(max) > 0))
		.Unless(actual, $"is in range of {min} and {max}");
}