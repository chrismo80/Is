using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Collections
{
	/// <summary>
	/// Asserts that the sequence is empty.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEmpty<T>(this IEnumerable<T> actual) => Check
		.That(!actual.Any())
		.Unless(actual, "is not empty");

	/// <summary>
	/// Asserts that all elements in the sequence are unique.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsUnique<T>(this IEnumerable<T> actual) => Check
		.That(actual.HasDuplicate(), result => !result.Yes, result => result.Duplicate)
		.Unless(actual, "is containing a duplicate");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence contains
	/// all the specified <paramref name="expected"/> elements.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected)
		where T : notnull => Check
		.That(actual.Diff(expected), result => result.Missing.Length == 0, result => result.Missing)
		.Unless(actual, "is not containing");

	/// <summary>
	/// Asserts that all elements in the <paramref name="actual"/> collection
	/// are present in the <paramref name="expected"/> collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected)
		where T : notnull => Check
		.That(actual.Diff(expected), result => result.Unexpected.Length == 0, result => result.Unexpected)
		.Unless(expected, "is not containing");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
		where T : notnull => Check
		.That(actual.Diff(expected),
			result => result.Missing.Length == 0 && result.Unexpected.Length == 0,
			result => $"missing {result.Missing.FormatValue()} and having {result.Unexpected.FormatValue()}")
		.Unless(actual, "is");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> dictionary matches
	/// the <paramref name="expected"/> dictionary ignoring order.
	/// Optional predicate can be used to ignore specific keys.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<TKey, T>(this IDictionary<TKey, T> actual, IDictionary<TKey, T> expected, Func<TKey, bool>? ignoreKeys = null)
		where TKey : notnull => Check
		.That(actual.Diffs(expected, ignoreKeys), diffs => diffs.Count == 0, diffs => diffs)
		.Unless("object is not matching");
}