using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
internal static class CollectionParser
{
	internal static (bool Result, T Duplicate) HasDuplicate<T>(this IEnumerable<T> items)
	{
		var set = new HashSet<T>();

		foreach (var item in items)
		{
			if (!set.Add(item))
				return (true, item);
		}

		return (false, default);
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