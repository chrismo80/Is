using Is.Core;
using Is.Tools;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

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
	public static bool IsUnique<T>(this IEnumerable<T> actual)
	{
		var (yes, duplicate) = actual.HasDuplicate();

		return Check
			.That(!yes)
			.Unless(actual, "is containing a duplicate", duplicate);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence contains
	/// all the specified <paramref name="expected"/> elements.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull
	{
		var (missing, _) = actual.Diff(expected);

		return Check
			.That(missing.Length == 0)
			.Unless(actual, "is not containing", missing);
	}

	/// <summary>
	/// Asserts that all elements in the <paramref name="actual"/> collection
	/// are present in the <paramref name="expected"/> collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull
	{
		var (_, unexpected) = actual.Diff(expected);

		return Check
			.That(unexpected.Length == 0)
			.Unless(unexpected, "is not in", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected) where T : notnull
	{
		var (missing, unexpected) = actual.Diff(expected);

		return Check
			.That(missing.Length == 0 && unexpected.Length == 0)
			.Unless(actual, $"is missing {missing.FormatValue()} and having {unexpected.FormatValue()}");
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> dictionary matches
	/// the <paramref name="expected"/> dictionary ignoring order.
	/// Optional predicate can be used to ignore specific keys.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<TKey, T>(this IDictionary<TKey, T> actual, IDictionary<TKey, T> expected, Func<TKey, bool>? ignoreKeys = null) where TKey : notnull
	{
		var diffs = actual.Diffs(expected, ignoreKeys);

		return Check
			.That(diffs.Count == 0)
			.Unless("object is not matching", diffs);
	}
}