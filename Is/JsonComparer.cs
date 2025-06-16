using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Is;

public static class JsonComparer
{
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

	internal static List<string> DifferencesTo(this string actualJson, string expectedJson) =>
		actualJson.ToJsonNode().CompareTo(expectedJson.ToJsonNode(), "", []);

	private static JsonNode? ToJsonNode(this string json) =>
		JsonNode.Parse(json);

	private static List<string> CompareTo(this JsonNode? actual, JsonNode? expected, string path, List<string> diffs)
	{
		if (path.Count(c => c is '.' or '[') > 100)
		{
			diffs.Add($"{path.Color(100)}: path too deep (length limit exceeded)");
			return diffs;
		}

		if (actual != null && expected != null)
			return (actual, expected) switch
			{
				(JsonValue, JsonValue) => Compare(actual, expected, path, diffs),
				(JsonObject actualObj, JsonObject expectedObj) => Compare(actualObj, expectedObj, path, diffs),
				(JsonArray actualArr, JsonArray expectedArr) => Compare(actualArr, expectedArr, path, diffs),
				_ => UnHandledNodeType(expected, path, diffs)
			};

		if (actual == null || expected == null)
			diffs.Add($"{path}: {actual.Format().Color(91)} is not {expected.Format().Color(92)}");

		return diffs;
	}

	private static List<string> Compare(JsonNode actual, JsonNode expected, string path, List<string> diffs)
	{
		if (actual.ToJsonString() != expected.ToJsonString())
			diffs.Add($"{path}: " + actual.ToJsonString().Color(91) + " is not " + expected.ToJsonString().Color(92));

		return diffs;
	}

	private static List<string> Compare(JsonObject actual, JsonObject expected, string path, List<string> diffs)
	{
		foreach (var prop in expected)
		{
			actual.TryGetPropertyValue(prop.Key, out var actualNode);
			actualNode.CompareTo(prop.Value, path == "" ? prop.Key : $"{path}.{prop.Key}", diffs);
		}

		return diffs;
	}

	private static List<string> Compare(JsonArray actual, JsonArray expected, string path, List<string> diffs)
	{
		if (expected.Count == actual.Count)
		{
			for (int i = 0; i < expected.Count; i++)
				actual[i].CompareTo(expected[i], $"{path}[{i}]", diffs);
		}
		else
			diffs.Add($"{path}: count mismatch ({actual.Count.Color(91)} is not {expected.Count.Color(92)})");

		return diffs;
	}

	private static List<string> UnHandledNodeType(JsonNode expected, string path, List<string> diffs)
	{
		diffs.Add($"{path}: unhandled json node type {expected.GetType().Name}");

		return diffs;
	}

	private static string Format(this JsonNode? node) =>
		node?.ToJsonString() ?? "<NULL>";
}