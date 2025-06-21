using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class ReflectionParser
{
	private const int MAX_RECURSION_DEPTH = 20;
	private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	internal static Dictionary<string, object?> Parse(this object sut) =>
		sut.Parse([], []);

	private static Dictionary<string, object?> Parse(this object? sut, HashSet<object> visited, Dictionary<string, object?> result, string path = "", int depth = 0)
	{
		if (depth > MAX_RECURSION_DEPTH)
			return result.AddItem(path, $"path too deep (length limit {depth} exceeded)");

		return sut switch
		{
			null => result.AddItem(path, sut),
			IDictionary dict => ParseDictionary(dict, visited, result, path, depth + 1),
			IEnumerable enumerable and not string => ParseEnumerable(enumerable, visited, result, path, depth + 1),
			_ => ParseDefault(sut, visited, result, path, depth)
		};
	}

	private static Dictionary<string, object?> ParseDefault(object me, HashSet<object> visited, Dictionary<string, object?> result, string path, int depth)
	{
		var type = me.GetType();

		if (!type.IsValueType && !visited.Add(me))
			return result;

		if (IsSimple(Nullable.GetUnderlyingType(type) ?? type))
			return result.AddItem(path, me);

		foreach (var prop in type.GetProperties(FLAGS).Where(p => p.GetIndexParameters().Length == 0 && p.CanRead))
			prop.GetValue(me).Parse(visited, result, path.Deeper(prop.Name), depth + 1);

		foreach (var field in type.GetFields(FLAGS).Where(f => !f.Name.StartsWith('<')))
			field.GetValue(me).Parse(visited, result, path.Deeper(field.Name), depth + 1);

		return result;
	}

	private static Dictionary<string, object?> ParseEnumerable(IEnumerable enumerable, HashSet<object> visited, Dictionary<string, object?> result, string path, int depth)
	{
		var list = enumerable.Cast<object?>().ToList();

		for (int i = 0; i < list.Count; i++)
			list[i].Parse(visited, result, $"{path}[{i}]", depth);

		return result;
	}

	private static Dictionary<string, object?> ParseDictionary(IDictionary dict, HashSet<object> visited, Dictionary<string, object?> result, string path, int depth)
	{
		foreach (var key in dict.Keys)
			dict[key].Parse(visited, result, $"{path}[{key}]", depth);

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