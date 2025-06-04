using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Collections
{
	/// <summary>
	/// Asserts that the sequence is empty.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEmpty<T>(this IEnumerable<T> actual)
	{
		if(!actual.Any())
			return true;

		throw new NotException(actual, "is not empty");
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
				throw new NotException(actual, "is containing a duplicate", item);
		}

		return true;
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence contains
	/// all the specified <paramref name="expected"/> elements.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining<T>(this IEnumerable<T> actual, params T[] expected)
		where T : notnull
	{
		var (missing, _) = actual.Diff(expected);

		if (missing.Length == 0)
			return true;

		throw new NotException(actual, "is not containing", missing);
	}

	/// <summary>
	/// Asserts that all elements in the <paramref name="actual"/> collection
	/// are present in the <paramref name="expected"/> collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsIn<T>(this IEnumerable<T> actual, params T[] expected)
		where T : notnull
	{
		var (_, unexpected) = actual.Diff(expected);

		if (unexpected.Length == 0)
			return true;

		throw new NotException(unexpected, "is not in", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> sequence matches
	/// the <paramref name="expected"/> sequence ignoring item order.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
		where T : notnull
	{
		var (missing, unexpected) = actual.Diff(expected);

		if (missing.Length == 0 && unexpected.Length == 0)
			return true;

		throw new NotException(actual, $"is missing {missing.FormatValue()} and having {unexpected.FormatValue()}");
	}

	private static (T[] Missing, T[] Unexpected) Diff<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
		where T : notnull
	{
		var histogram = new Dictionary<T, int>();

		histogram.CountItems(actual, 1);
		histogram.CountItems(expected, -1);

		return (histogram.Filter(c => c < 0), histogram.Filter(c => c > 0));
	}

	private static void CountItems<T>(this Dictionary<T, int> dict, IEnumerable<T> source, int increment)
		where T : notnull
	{
		foreach (var item in source)
			dict[item] = dict.GetValueOrDefault(item) + increment;
	}

	private static T[] Filter<T>(this Dictionary<T, int> dict, Func<int, bool> predicate) where T : notnull =>
		dict.Where(kvp => predicate(kvp.Value)).Select(kvp => kvp.Key).ToArray();
}