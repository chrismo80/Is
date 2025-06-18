using System.Collections;
using System.Reflection;

namespace Is.Parser;

internal static class ReflectionParser
{
	private const int MAX_RECURSION_DEPTH = 20;
	private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	internal static Dictionary<string, object?> Parse(this object sut) =>
		sut.Parse("", [], []);

	private static Dictionary<string, object?> Parse(this object? sut, string path, HashSet<object> visited, Dictionary<string, object?> result)
	{
		if (path.Count(c => c is '.' or '[') > MAX_RECURSION_DEPTH)
			return result.AddItem(path, "path too deep (length limit exceeded)");

		return sut switch
		{
			null => result.AddItem(path, sut),
			IDictionary dict => ParseDictionary(dict, path, visited, result),
			IEnumerable enumerable and not string => ParseEnumerable(enumerable, path, visited, result),
			_ => ParseDefault(sut, path, visited, result)
		};
	}

	private static Dictionary<string, object?> ParseDefault(object me, string path, HashSet<object> visited, Dictionary<string, object?> result)
	{
		var type = me.GetType();

		if (!type.IsValueType && !visited.Add(me))
			return result;

		if (IsSimple(Nullable.GetUnderlyingType(type) ?? type))
			return result.AddItem(path, me);

		foreach (var prop in type.GetProperties(FLAGS).Where(p => p.GetIndexParameters().Length == 0 && p.CanRead))
			prop.GetValue(me).Parse(path.Deeper(prop.Name), visited, result);

		foreach (var field in type.GetFields(FLAGS).Where(f => !f.Name.StartsWith('<')))
			field.GetValue(me).Parse(path.Deeper(field.Name), visited, result);

		return result;
	}

	private static Dictionary<string, object?> ParseEnumerable(IEnumerable enumerable, string path, HashSet<object> visited, Dictionary<string, object?> result)
	{
		var list = enumerable.Cast<object?>().ToList();

		for (int i = 0; i < list.Count; i++)
			list[i].Parse($"{path}[{i}]", visited, result);

		return result;
	}

	private static Dictionary<string, object?> ParseDictionary(IDictionary dict, string path, HashSet<object> visited, Dictionary<string, object?> result)
	{
		foreach (var key in dict.Keys)
			dict[key].Parse($"{path}[{key}]", visited, result);

		return result;
	}

	private static Dictionary<string, object?> AddItem(this Dictionary<string, object?> data, string path, object? item)
	{
		data[path] = item;
		return data;
	}

	private static string Deeper(this string path, string next) =>
		path == "" ? next : $"{path}.{next}";

	private static bool IsSimple(this Type type) =>
		Type.GetTypeCode(type) != TypeCode.Object || type.IsEnum;
}