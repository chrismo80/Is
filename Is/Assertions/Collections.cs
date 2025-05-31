namespace Is.Assertions;

public static class Collections
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