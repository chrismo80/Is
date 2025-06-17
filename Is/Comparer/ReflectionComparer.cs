using System.Collections;
using System.Reflection;

namespace Is;

public static class ReflectionComparer
{
	private const int RECURSION_DEPTH = 100;
	private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	internal static List<string> DifferencesTo(this object? actual, object? expected) =>
		CompareTo(actual, expected, "", [], []);

	private static List<string> CompareTo(this object? actual, object? expected, string path, HashSet<object> visited, List<string> diffs)
	{
		if (path.Count(c => c is '.' or '[') > RECURSION_DEPTH)
		{
			diffs.Add($"{path.Color(100)}: path too deep (length limit exceeded)");
			return diffs;
		}

		if (actual != null && expected != null)
			return (actual, expected) switch
			{
				(string a, string e) => CompareObjects(a, e, path, visited, diffs),
				(IDictionary a, IDictionary e) => Compare(a, e, path, visited, diffs),
				(IEnumerable a, IEnumerable e) => Compare(a, e, path, visited, diffs),
				_ => CompareObjects(actual, expected, path, visited, diffs)
			};

		if (actual == null || expected == null)
			diffs.Add($"{path}: {actual.FormatValue().Simply("is not", expected.FormatValue())}");

		return diffs;
	}

	private static List<string> CompareObjects(object actual, object expected, string path, HashSet<object> visited, List<string> diffs)
	{
		var type = actual.GetType();

		if (!type.IsValueType && !visited.Add(actual))
			return diffs;

		if (IsSimple(Nullable.GetUnderlyingType(type) ?? type))
		{
			if (!actual.IsExactlyEqualTo(expected))
				diffs.Add($"{path}: {actual.FormatValue().Simply("is not", expected.FormatValue())}");

			return diffs;
		}

		foreach (var prop in type.GetProperties(FLAGS).Where(p => p.GetIndexParameters().Length == 0 && p.CanRead))
			CompareTo(prop.GetValue(actual), prop.GetValue(expected), path == "" ? prop.Name : $"{path}.{prop.Name}", visited, diffs);

		foreach (var field in type.GetFields(FLAGS).Where(f => !f.Name.StartsWith('<')))
			CompareTo(field.GetValue(actual), field.GetValue(expected), path == "" ? field.Name : $"{path}.{field.Name}", visited, diffs);

		return diffs;
	}

	private static List<string> Compare(IEnumerable actual, IEnumerable expected, string path, HashSet<object> visited, List<string> diffs)
	{
		var (actualArr, expectedArr) = (actual.Cast<object?>().ToList(), expected.Cast<object?>().ToList());

		if (actualArr.Count == expectedArr.Count)
		{
			for (int i = 0; i < expectedArr.Count; i++)
				actualArr[i].CompareTo(expectedArr[i], $"{path}[{i}]", visited, diffs);
		}
		else
			diffs.Add($"{path}: count mismatch ({actualArr.Count.Simply("is not", expectedArr.Count)})");

		return diffs;
	}

	private static List<string> Compare(IDictionary actual, IDictionary expected, string path, HashSet<object> visited, List<string> diffs)
	{
		foreach (var key in new HashSet<object>(actual.Keys.Cast<object>().Concat(expected.Keys.Cast<object>())))
			CompareTo(actual.GetValue(key), expected.GetValue(key), $"{path}[{key.FormatValue()}]", visited, diffs);

		return diffs;
	}

	private static object? GetValue(this IDictionary dict, object key) =>
		dict.Contains(key) ? dict[key] : null;

	private static bool IsSimple(this Type type) =>
		Type.GetTypeCode(type) != TypeCode.Object || type.IsEnum;
}