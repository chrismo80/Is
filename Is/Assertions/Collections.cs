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
	/// Asserts that the sequence is not empty.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotEmpty<T>(this IEnumerable<T> actual) => Check
		.That(actual.Any())
		.Unless(actual, "is empty");

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

	/// <summary>Asserts that the sequence contains the specified elements.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining<T>(this IEnumerable<T> actual, T expected) => Check
		.That(actual.Contains(expected))
		.Unless(actual, "is not containing", expected);

	/// <summary>Asserts that the sequence does not contain the specified elements.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotContaining<T>(this IEnumerable<T> actual, T expected) => Check
		.That(!actual.Contains(expected))
		.Unless(actual, "is containing", expected);

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

	/// <summary>Checks that the specified element is contained within the given sequence.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsIn<T>(this T actual, IEnumerable<T> expected) => Check
		.That(expected.Contains(actual))
		.Unless(actual, "is not in", expected);

	/// <summary>Checks that the specified element is not contained within the given sequence.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotIn<T>(this T actual, IEnumerable<T> expected) => Check
		.That(!expected.Contains(actual))
		.Unless(actual, "is in", expected);

	/// <summary>Asserts that the sequence is ordered in ascending order.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsOrdered<T>(this IEnumerable<T> actual) where T : IComparable<T> =>
		VerifyOrder(actual, (next, prev) => next.IsAtLeast(prev));

	/// <summary>Asserts that the sequence is ordered in descending order.</summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsOrderedDescending<T>(this IEnumerable<T> actual) where T : IComparable<T> =>
		VerifyOrder(actual, (next, prev) => next.IsAtMost(prev));

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order by using Default Equality comparer of <typeparamref name="T" />.
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
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order by using Deeply Equality comparer of <typeparamref name="T" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsDeeplyEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected,
		Func<string, bool>? ignorePaths = null) where T : notnull
	{
		var (missing, unexpected) = actual.Diff(expected, new DeepEqualityComparer<T>(ignorePaths));

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
		var failures = actual.Diffs(expected, ignoreKeys);

		return failures.Count == 0 ? Assertion.Passed() :
			Assertion.Failed<bool>(new Failure($"{failures.Count} mismatches", null, null, failures.ToList()));
	}

	private static (bool Yes, T? Duplicate) HasDuplicate<T>(this IEnumerable<T> items)
	{
		var set = new HashSet<T>();

		foreach (var item in items)
		{
			if (!set.Add(item))
				return (true, item);
		}

		return (false, default);
	}

	internal static List<Failure> Diffs<TKey, T>(this IDictionary<TKey, T> actual, IDictionary<TKey, T> expected, Func<TKey, bool>? ignoreKeys = null)
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

		var diffs = missing.Zip(unexpected, (m, u) => new Failure($"{u.Key}: {u.Value.Simply("is not", m.Value)}", u.Value, m.Value, null, true)).ToList();

		if (missingKeys.Length == 0 && unexpectedKeys.Length == 0 && diffs.Count == 0)
			return [];

		var failures = diffs
			.Concat(missingKeys.Select(k => new Failure($"{k.Color(100)}: missing", null, expected[k], null, true)))
			.Concat(unexpectedKeys.Select(k => new Failure($"{k.Color(100)}: unexpected", actual[k], null, null, true)));

		return failures.ToList();
	}

	private static IEnumerable<T> Ignore<T>(this IEnumerable<T> items, Func<T, bool>? predicate) where T : notnull =>
		items.Where(item => !(predicate?.Invoke(item) ?? false));

	private static (T[] Missing, T[] Unexpected) Diff<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T>? comparer = null) where T : notnull
	{
		var histogram = new Dictionary<T, int>(comparer);

		histogram.CountItems(actual, 1);
		histogram.CountItems(expected, -1);

		return (histogram.Filter(c => c < 0), histogram.Filter(c => c > 0));
	}

	private static void CountItems<T>(this Dictionary<T, int> dict, IEnumerable<T> source, int increment) where T : notnull
	{
		foreach (var item in source)
			dict[item] = dict.GetValueOrDefault(item) + increment;
	}

	private static T[] Filter<T>(this Dictionary<T, int> dict, Func<int, bool> predicate) where T : notnull =>
		dict.Where(kvp => predicate(kvp.Value)).Select(kvp => kvp.Key).ToArray();

	private static bool VerifyOrder<T>(IEnumerable<T> actual, Func<T, T, bool> check) where T : IComparable<T>
	{
		using var enumerator = actual.GetEnumerator();

		if (!enumerator.MoveNext())
			return Assertion.Passed();

		var prev = enumerator.Current;

		while (enumerator.MoveNext())
		{
			var next = enumerator.Current;

			check(next, prev);

			prev = next;
		}

		return Assertion.Passed();
	}
}