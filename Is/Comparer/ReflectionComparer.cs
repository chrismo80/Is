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
		var (actualType, expectedType) = (actual.GetType(), expected.GetType());

		if (actualType != expectedType && path != "")
		{
			diffs.Add($"{path}: type mismatch ({actualType.Name.Simply("is no", expectedType.Name)})");
			return diffs;
		}

		if (!actualType.IsValueType && !visited.Add(actual))
			return diffs;

		if (IsSimple(Nullable.GetUnderlyingType(actualType) ?? actualType))
		{
			if (!actual.IsExactlyEqualTo(expected))
				diffs.Add($"{path}: {actual.FormatValue().Simply("is not", expected.FormatValue())}");
			return diffs;
		}

		var actualProps = actualType.GetPropertyNames();
		var expectedProps = expectedType.GetPropertyNames();

		foreach (var name in new HashSet<string>(actualProps.Keys.Concat(expectedProps.Keys)))
		{
			if (actualProps.TryGetValue(name, out var aInfo) && expectedProps.TryGetValue(name, out var eInfo))
				aInfo.GetValue(actual).CompareTo(eInfo.GetValue(expected), path.Deeper(name), visited, diffs);
			else
			{
				var state = actualProps.ContainsKey(name) ? "unexpected" : "missing";

				diffs.Add($"{path.Deeper(name).Color(100)}: {state} property");
			}
		}

		var actualFields = actualType.GetFieldNames();
		var expectedFields = expectedType.GetFieldNames();

		foreach (var name in new HashSet<string>(actualFields.Keys.Concat(expectedFields.Keys)))
		{
			if (actualFields.TryGetValue(name, out var aInfo) && expectedFields.TryGetValue(name, out var eInfo))
				aInfo.GetValue(actual).CompareTo(eInfo.GetValue(expected), path.Deeper(name), visited, diffs);
			else
			{
				var state = actualFields.ContainsKey(name) ? "unexpected" : "missing";

				diffs.Add($"{path.Deeper(name).Color(100)}: {state} field");
			}
		}

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

	private static  Dictionary<string,PropertyInfo> GetPropertyNames(this Type type) =>
		type.GetProperties(FLAGS).Where(p => p.GetIndexParameters().Length == 0 && p.CanRead).ToDictionary(p => p.Name);

	private static  Dictionary<string,FieldInfo> GetFieldNames(this Type type) =>
		type.GetFields(FLAGS).Where(f => !f.Name.StartsWith('<')).ToDictionary(f => f.Name);

	private static string Deeper(this string path, string next) =>
		path == "" ? next : $"{path}.{next}";

	private static object? GetValue(this IDictionary dict, object key) =>
		dict.Contains(key) ? dict[key] : null;

	private static bool IsSimple(this Type type) =>
		Type.GetTypeCode(type) != TypeCode.Object || type.IsEnum;
}