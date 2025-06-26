using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class JsonParser
{
	private static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = true };

	internal static string ToJson<T>(this T me, JsonSerializerOptions? options = null) =>
		JsonSerializer.Serialize(me, options ?? DefaultOptions);

	internal static T? FromJson<T>(this string json) =>
		JsonSerializer.Deserialize<T>(json, DefaultOptions);

	internal static Dictionary<string, object?> ParseJson(this string json) =>
		JsonNode.Parse(json).Parse([]);

	private static Dictionary<string, object?> Parse(this JsonNode? node, Dictionary<string, object?> result, string path = "", int depth = 0)
	{
		if (depth > Configuration.Active.MaxRecursionDepth)
			return result.AddItem(path, $"path too deep (length limit {depth} exceeded)");

		return node switch
		{
			null => result.AddItem(path, null),
			JsonValue value => result.AddItem(path, value.GetValue<JsonElement>().ToValue()),
			JsonObject obj => ParseObject(obj, result, path, depth + 1),
			JsonArray array => ParseArray(array, result, path, depth + 1),
			_ => result.AddItem(path, "unhandled json node type"),
		};
	}

	private static Dictionary<string, object?> ParseArray(JsonArray array, Dictionary<string, object?> result, string path, int depth)
	{
		for (int i = 0; i < array.Count; i++)
			array[i].Parse(result, $"{path}[{i}]", depth);

		return result;
	}

	private static Dictionary<string, object?> ParseObject(JsonObject obj, Dictionary<string, object?> result, string path, int depth)
	{
		foreach (var node in obj)
			node.Value.Parse(result, path.Deeper(node.Key), depth);

		return result;
	}

	private static object? ToValue(this JsonElement element) => element.ValueKind switch
	{
		JsonValueKind.Number when element.TryGetInt64(out var l) => l,
		JsonValueKind.Number => element.GetDouble(),
		JsonValueKind.String => element.GetString(),
		JsonValueKind.True => true,
		JsonValueKind.False => false,
		JsonValueKind.Null => null,
		_ => throw new NotSupportedException($"Unsupported value kind: {element.ValueKind}")
	};

	private static Dictionary<string, object?> AddItem(this Dictionary<string, object?> data, string path, object? item)
	{
		data[path] = item;
		return data;
	}

	private static string Deeper(this string path, string next) =>
		path == "" ? next : $"{path}.{next}";
}

public static class JsonFileHelper
{
	/// <summary>
	/// Serializes an object <paramref name="obj"/> to a JSON file to <paramref name="filename"/>
	/// </summary>
	public static void SaveJson<T>(this T obj, string filename) =>
		File.WriteAllText(filename, obj.ToJson(), Encoding.UTF8);

	/// <summary>
	/// Deserializes an object to type <typeparamref name="T" /> from a JSON file at <paramref name="filename"/>
	/// </summary>
	public static T? LoadJson<T>(this string filename) =>
		File.ReadAllText(filename, Encoding.UTF8).FromJson<T>();
}