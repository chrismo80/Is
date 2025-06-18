using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Is.Parser;

public static class JsonComparer
{
	private const int MAX_RECURSION_DEPTH = 20;
	private static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = true };

	/// <summary>
	/// Serializes an object <paramref name="me"/> to a JSON file to <paramref name="filename"/>
	/// </summary>
	public static void ToJsonFile<T>(this T me, string filename) =>
		File.WriteAllText(filename, me.ToJson(), Encoding.UTF8);

	/// <summary>
	/// Deserializes an object to type <typeparamref name="T" /> from a JSON file at <paramref name="filename"/>
	/// </summary>
	public static T? FromJsonFile<T>(this string filename) =>
		File.ReadAllText(filename, Encoding.UTF8).FromJson<T>();

	internal static string ToJson<T>(this T me, JsonSerializerOptions? options = null) =>
		JsonSerializer.Serialize(me, options ?? DefaultOptions);

	internal static T? FromJson<T>(this string json) =>
		JsonSerializer.Deserialize<T>(json, DefaultOptions);

	internal static Dictionary<string, object?> ParseJson(this string json) =>
		JsonNode.Parse(json).Parse("", []);

	private static Dictionary<string, object?> Parse(this JsonNode? node, string path, Dictionary<string, object?> result)
	{
		if (path.Count(c => c is '.' or '[') > MAX_RECURSION_DEPTH)
			return result.AddItem(path, "path too deep (length limit exceeded)");

		return node switch
		{
			null => result.AddItem(path, null),
			JsonValue value => result.AddItem(path, value.GetValue<JsonElement>().ToValue()),
			JsonObject obj => ParseObject(obj, path, result),
			JsonArray array => ParseArray(array, path, result),
			_ => result.AddItem(path, "unhandled json node type"),
		};
	}

	private static Dictionary<string, object?> ParseArray(JsonArray array, string path, Dictionary<string, object?> result)
	{
		for (int i = 0; i < array.Count; i++)
			array[i].Parse($"{path}[{i}]", result);

		return result;
	}

	private static Dictionary<string, object?> ParseObject(JsonObject obj, string path, Dictionary<string, object?> result)
	{
		foreach (var node in obj)
			node.Value.Parse(path.Deeper(node.Key), result);

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