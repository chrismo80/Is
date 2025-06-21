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
	public static bool IsUnique<T>(this IEnumerable<T> actual)
	{
		if(actual.HasDuplicate() is { Result: true } result)
			return Assertion.Failed<bool>(actual, "is containing a duplicate", result.Duplicate);

		return Assertion.Passed();
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence contains
	/// all the specified <paramref name="expected"/> elements.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull
	{
		var (missing, _) = actual.Diff(expected);

		if (missing.Length == 0)
			return Assertion.Passed();

		return Assertion.Failed<bool>(actual, "is not containing", missing);
	}

	/// <summary>
	/// Asserts that all elements in the <paramref name="actual"/> collection
	/// are present in the <paramref name="expected"/> collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected) where T : notnull
	{
		var (_, unexpected) = actual.Diff(expected);

		if (unexpected.Length == 0)
			return Assertion.Passed();

		return Assertion.Failed<bool>(unexpected, "is not in", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected) where T : notnull
	{
		var (missing, unexpected) = actual.Diff(expected);

		if (missing.Length == 0 && unexpected.Length == 0)
			return Assertion.Passed();

		return Assertion.Failed<bool>(actual, $"is missing {missing.FormatValue()} and having {unexpected.FormatValue()}");
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> dictionary matches
	/// the <paramref name="expected"/> dictionary ignoring order.
	/// Optional predicate can be used to ignore specific keys.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<TKey, T>(this IDictionary<TKey, T> actual, IDictionary<TKey, T> expected, Func<TKey, bool>? ignoreKeys = null)
		where TKey : notnull
	{
		if (ignoreKeys is not null)
		{
			actual = actual.Ignore(kvp => ignoreKeys(kvp.Key)).ToDictionary();
			expected = expected.Ignore(kvp => ignoreKeys(kvp.Key)).ToDictionary();
		}

		var (missingKeys, unexpectedKeys) = actual.Keys.Diff(expected.Keys);

		var (missing, unexpected) = actual.Where(kvp => !unexpectedKeys.Contains(kvp.Key))
			.Diff(expected.Where(kvp => !missingKeys.Contains(kvp.Key)));

		var diffs = missing.Zip(unexpected, (m, u) => $"{u.Key}: {u.Value.Simply("is not", m.Value)}").ToList();

		if(missingKeys.Length == 0 && unexpectedKeys.Length == 0 && diffs.Count == 0)
			return Assertion.Passed();

		var messages = diffs
			.Concat(missingKeys.Select(k => $"{k.Color(100)}: missing {expected[k].Format()}"))
			.Concat(unexpectedKeys.Select(k => $"{k.Color(100)}: unexpected"));

		return Assertion.Failed<bool>("object is not matching", messages.ToList());
	}
}