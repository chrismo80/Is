using System.Text;
using Is.Parser;

namespace Is;

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