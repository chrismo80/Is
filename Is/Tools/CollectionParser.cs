using Is.Core;
using System.Diagnostics;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class CollectionParser
{
	internal static (bool Yes, T Duplicate) HasDuplicate<T>(this IEnumerable<T> items)
	{
		var set = new HashSet<T>();

		foreach (var item in items)
		{
			if (!set.Add(item))
				return (true, item);
		}

		return (false, default);
	}

	internal static List<string> Diffs<TKey, T>(this IDictionary<TKey, T> actual, IDictionary<TKey, T> expected, Func<TKey, bool>? ignoreKeys = null)
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

		if (missingKeys.Length == 0 && unexpectedKeys.Length == 0 && diffs.Count == 0)
			return [];

		var messages = diffs
			.Concat(missingKeys.Select(k => $"{k.Color(100)}: missing {expected[k].Format()}"))
			.Concat(unexpectedKeys.Select(k => $"{k.Color(100)}: unexpected"));

		return messages.ToList();
	}

	internal static IEnumerable<T> Ignore<T>(this IEnumerable<T> items, Func<T, bool>? predicate) where T : notnull =>
		items.Where(item => !(predicate?.Invoke(item) ?? false));

	internal static (T[] Missing, T[] Unexpected) Diff<T>(this IEnumerable<T> actual, IEnumerable<T> expected) where T : notnull
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
}