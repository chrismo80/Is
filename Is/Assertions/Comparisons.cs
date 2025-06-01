using System.Numerics;

namespace Is.Assertions;

public static class Comparisons
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> floating point
	/// is approximately equal to <paramref name="expected"/>.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IFloatingPoint{TSelf}"/>.</typeparam>
	public static bool IsApproximately<T>(this T actual, T expected, T epsilon)
		where T : IFloatingPoint<T>
	{
		if(T.Abs(actual - expected) <= epsilon * T.Max(T.One, T.Abs(expected)))
			return true;

		throw new NotException(actual, "is not close enough to", expected);
	}

	/// <summary>
	/// default epsilon is 1e-6.
	/// </summary>
	public static bool IsApproximately<T>(this T actual, T expected) where T : IFloatingPoint<T> =>
		actual.IsApproximately(expected, T.CreateChecked(1e-6));

	/// <summary>
	/// Asserts that the actual value is greater
	/// than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsGreaterThan<T>(this T actual, T other)
		where T : IComparable<T>
	{
		if(actual.CompareTo(other) > 0)
			return true;

		throw new NotException(actual, "is not greater than", other);
	}

	/// <summary>
	/// Asserts that the actual value is smaller
	/// than the given <paramref name="other" /> value.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsSmallerThan<T>(this T actual, T other)
		where T : IComparable<T>
	{
		if(actual.CompareTo(other) < 0)
			return true;

		throw new NotException(actual, "is not smaller than", other);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> value
	/// is between <paramref name="min"/> and <paramref name="max"/> exclusive bounds.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsBetween<T>(this T actual, T min, T max) where T : IComparable<T> =>
		actual.IsGreaterThan(min) && actual.IsSmallerThan(max);

	/// <summary>
	/// Asserts that the actual value is not between
	/// the specified <paramref name="min"/> and <paramref name="max"/> exclusive bounds.
	/// </summary>
	/// <typeparam name="T">A type that implements <see cref="IComparable"/>.</typeparam>
	public static bool IsNotBetween<T>(this T actual, T min, T max)
		where T : IComparable<T>
	{
		if(actual.CompareTo(max) > 0 || actual.CompareTo(min) < 0)
			return true;

		throw new NotException(actual, $"is between {min} and {max}");
	}
}