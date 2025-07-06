using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class DateTimes
{
	/// <summary>
	/// Asserts that the difference between two <see cref="DateTime"/>
	/// is within the specified <paramref name="tolerance"/>.
	/// </summary>
	public static bool IsApproximately(this DateTime actual, DateTime expected, TimeSpan tolerance) =>
		(actual - expected).Duration().IsAtMost(tolerance);

	/// <summary>
	/// Asserts that the difference between two <see cref="TimeSpan"/>
	/// is within the specified <paramref name="tolerance"/>.
	/// </summary>
	public static bool IsApproximately(this TimeSpan actual, TimeSpan expected, TimeSpan tolerance) =>
		(actual - expected).Duration().IsAtMost(tolerance);

	/// <summary>
	/// Determines whether the specified <paramref name="actual"/> date and
	/// <paramref name="other"/> date are on the same calendar day.
	/// </summary>
	public static bool IsSameDay(this DateTime actual, DateTime other) =>
		actual.Date.IsExactly(other.Date);

	/// <summary>
	/// Checks that the given <paramref name="actual"/> date is older than the specified number of
	/// <paramref name="years"/> relative to the reference date <paramref name="date"/> or today if not provided.
	/// </summary>
	public static bool IsOlderThan(this DateTime actual, int years, DateTime date) =>
		actual.GetAgeTo(date).IsAtLeast(years);

	/// <summary>
	/// Checks that the given <paramref name="actual"/> date is older than
	/// the specified number of <paramref name="years"/>.
	/// </summary>
	public static bool IsOlderThan(this DateTime actual, int years) =>
		actual.IsOlderThan(years, DateTime.Today);

	private static int GetAgeTo(this DateTime birthDate, DateTime date)
	{
		int age = date.Year - birthDate.Year;

		if (birthDate.Date > date.AddYears(-age).Date)
			age--;

		return age;
	}
}