using System.Collections;
using System.Reflection;

namespace Is;

public static class ReflectionComparer
{
	private const int RECURSION_DEPTH = 20;
	private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	internal static List<string> DifferencesTo(this object? actual, object? expected) =>
		CompareTo(actual, expected, "", [], []);

	private static List<string> CompareTo(this object? actual, object? expected, string path, HashSet<object> visited, List<string> diffs)
	{
		if (path.Count(c => c is '.' or '[') > RECURSION_DEPTH)
			return diffs.AddMessage($"{path.Color(100)}: path too deep (length limit exceeded)");

		if (actual == null || expected == null)
			return diffs.AddMessage($"{path}: {actual.FormatValue().Simply("is not", expected.FormatValue())}");

		return (actual, expected) switch
		{
			(string a, string e) => CompareObjects(a, e, path, visited, diffs),
			(IDictionary a, IDictionary e) => Compare(a, e, path, visited, diffs),
			(IEnumerable a, IEnumerable e) => Compare(a, e, path, visited, diffs),
			_ => CompareObjects(actual, expected, path, visited, diffs)
		};
	}

	private static List<string> CompareObjects(object actual, object expected, string path, HashSet<object> visited, List<string> diffs)
	{
		var (actualType, expectedType) = (actual.GetType(), expected.GetType());

		if (!actualType.IsValueType && !visited.Add(actual))
			return diffs;

		if (IsSimple(Nullable.GetUnderlyingType(actualType) ?? actualType))
		{
			if (actualType != expectedType && path != "")
				return diffs.AddMessage($"{path}: type mismatch ({actualType.Name.Simply("is no", expectedType.Name)})");

			if (!actual.IsExactlyEqualTo(expected))
				diffs.Add($"{path}: {actual.FormatValue().Simply("is not", expected.FormatValue())}");

			return diffs;
		}

		var actualProps = actualType.GetPropertyNames();
		var expectedProps = expectedType.GetPropertyNames();

		diffs.AddRange(actualProps.Where(p => !expectedProps.ContainsKey(p.Key)).Select(n => $"{path.Deeper(n.Key).Color(100)}: unexpected property"));
		diffs.AddRange(expectedProps.Where(p => !actualProps.ContainsKey(p.Key)).Select(n => $"{path.Deeper(n.Key).Color(100)}: missing property"));

		foreach (var info in expectedProps.Where(p => actualProps.ContainsKey(p.Key)).ToList())
			actualProps[info.Key].GetValue(actual).CompareTo(expectedProps[info.Key].GetValue(expected), path.Deeper(info.Key), visited, diffs);

		var actualFields = actualType.GetFieldNames();
		var expectedFields = expectedType.GetFieldNames();

		diffs.AddRange(actualFields.Where(p => !expectedFields.ContainsKey(p.Key)).Select(n => $"{path.Deeper(n.Key).Color(100)}: unexpected field"));
		diffs.AddRange(expectedFields.Where(p => !actualFields.ContainsKey(p.Key)).Select(n => $"{path.Deeper(n.Key).Color(100)}: missing field"));

		foreach (var info in expectedFields.Where(f => actualFields.ContainsKey(f.Key)))
			actualFields[info.Key].GetValue(actual).CompareTo(expectedFields[info.Key].GetValue(expected), path.Deeper(info.Key), visited, diffs);

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

	private static  Dictionary<string, PropertyInfo> GetPropertyNames(this Type type) => type.GetProperties(FLAGS)
		.Where(p => p.GetIndexParameters().Length == 0 && p.CanRead).ToDictionary(p => p.Name);

	private static  Dictionary<string, FieldInfo> GetFieldNames(this Type type) => type.GetFields(FLAGS)
		.Where(f => !f.Name.StartsWith('<')).ToDictionary(f => f.Name);

	private static List<string> AddMessage(this List<string> diffs, string message)
	{
		diffs.Add(message);
		return diffs;
	}

	private static string Deeper(this string path, string next) =>
		path == "" ? next : $"{path}.{next}";

	private static object? GetValue(this IDictionary dict, object key) =>
		dict.Contains(key) ? dict[key] : null;

	private static bool IsSimple(this Type type) =>
		Type.GetTypeCode(type) != TypeCode.Object || type.IsEnum;
}