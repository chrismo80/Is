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
	public static bool IsEmpty<T>(this IEnumerable<T> actual)
	{
		if (!actual.Any())
			return true;

		return new NotException(actual, "is not empty").HandleFailure<bool>();
	}

	/// <summary>
	/// Asserts that all elements in the sequence are unique.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsUnique<T>(this IEnumerable<T> actual)
	{
		var set = new HashSet<T>();

		foreach (var item in actual)
		{
			if (!set.Add(item))
				return new NotException(actual, "is containing a duplicate", item).HandleFailure<bool>();
		}

		return true;
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
			return true;

		return new NotException(actual, "is not containing", missing).HandleFailure<bool>();
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
			return true;

		return new NotException(unexpected, "is not in", expected).HandleFailure<bool>();
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
			return true;

		return new NotException(actual, $"is missing {missing.FormatValue()} and having {unexpected.FormatValue()}").HandleFailure<bool>();
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

		var diffs = missing.Zip(unexpected, (m, u) => $"{u.Key}: {u.Value.FormatValue().Simply("is not", m.Value.FormatValue())}").ToList();

		if(missingKeys.Length == 0 && unexpectedKeys.Length == 0 && diffs.Count == 0)
			return true;

		var messages = diffs
			.Concat(missingKeys.Select(k => $"{k.Color(100)}: missing {expected[k].FormatValue()}"))
			.Concat(unexpectedKeys.Select(k => $"{k.Color(100)}: unexpected"));

		return new NotException("object is not matching", messages.ToList()).HandleFailure<bool>();
	}

	private static (T[] Missing, T[] Unexpected) Diff<T>(this IEnumerable<T> actual, IEnumerable<T> expected) where T : notnull
	{
		var histogram = new Dictionary<T, int>();

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

	private static IEnumerable<T> Ignore<T>(this IEnumerable<T> items, Func<T, bool>? predicate) where T : notnull =>
		items.Where(item => !(predicate?.Invoke(item) ?? false));
}