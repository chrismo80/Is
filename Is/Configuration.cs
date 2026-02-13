using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using Is.FailureObservers;
using Is.TestAdapters;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Is;

/// <summary>
/// Global configurations that control assertion behaviour.
/// </summary>
/// <remarks>
///
/// Can be set via <c>is.configuration.json</c>:
/// <code>
/// {
///	"AssertionObserver": "Is.FailureObservers.MarkDownObserver, Is",
///	"TestAdapter": "Is.TestAdapters.DefaultAdapter, Is",
///	"AppendCodeLine": true,
///	"ColorizeMessages": true,
///	"FloatingPointComparisonPrecision": 1E-06,
///	"MaxRecursionDepth": 20,
///	"ParsingFlags": 52
/// }
/// </code>
/// </remarks>
[DebuggerStepThrough]
public class Configuration
{
	const string ConfigFile = "is.configuration.json";
	internal static Configuration Default { get; } = ConfigFile.LoadJson<Configuration>() ?? new Configuration();

	public static Configuration Active => AssertionContext.Current?.Configuration ?? Default;

	/// <summary>
	/// Specifies the adapter responsible for integrating the assertion framework with external testing frameworks.
	/// Default is <see cref="DefaultAdapter"/>that is throwing <see cref="NotException"/> for single failures
	/// and a <see cref="AggregateException"/> for multiple failures.
	/// </summary>
	[JsonConverter(typeof(TypeConverter<ITestAdapter, DefaultAdapter>))]
	public ITestAdapter? TestAdapter { get; set; } = new DefaultAdapter();

	/// <summary>
	/// Observes all assertion evaluations — both passed and failed.
	/// Unlike <see cref="IAssertionObserver"/> which receives all assertions,
	/// individual observers can filter by the Passed property. Default is null (disabled).
	/// </summary>
	[JsonConverter(typeof(NullableTypeConverter<IAssertionObserver>))]
	public IAssertionObserver? AssertionObserver { get; set; }

	/// <summary>
	/// Makes code line info in <see cref="Failure"/> optional.
	/// </summary>
	public bool AppendCodeLine { get; set; } = true;

	/// <summary>
	/// Controls whether messages produced by assertions are colorized when displayed.
	/// Default is true, enabling colorization for better readability and visual distinction.
	/// </summary>
	public bool ColorizeMessages { get; set; } = true;

	/// <summary>
	/// Comparison precision used for floating point comparisons if not specified specifically.
	/// Default is 1e-6 (0.000001).
	/// </summary>
	public double FloatingPointComparisonPrecision { get; set; } = 1e-6;

	/// <summary>
	/// Controls the maximum depth of recursion when parsing deeply nested objects.
	/// Default is 20.
	/// </summary>
	public int MaxRecursionDepth { get; set; } = 20;

	/// <summary>
	/// Controls the binding flags to use when parsing deeply nested objects.
	/// Default is public | non-public | instance.
	/// </summary>
	public BindingFlags ParsingFlags { get; set; } = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	/// <summary>
	/// These options dictate aspects such as how JSON properties are written, ignored, or formatted,
	/// enabling fine-grained control over the serialization processes.
	/// </summary>
	[JsonIgnore]
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	internal Configuration Clone() => new()
	{
		TestAdapter = TestAdapter,
		AssertionObserver = AssertionObserver,
		AppendCodeLine = AppendCodeLine,
		ColorizeMessages = ColorizeMessages,
		FloatingPointComparisonPrecision = FloatingPointComparisonPrecision,
		MaxRecursionDepth = MaxRecursionDepth,
		ParsingFlags = ParsingFlags,
		JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions),
	};
}

file class TypeConverter<TInterface, T> : JsonConverter<TInterface>
	where TInterface : class where T : TInterface, new()
{
	public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		reader.GetString()?.ToType()?.ToInstance<TInterface>() ?? new T();

	public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.GetType().AssemblyQualifiedName);
}

file class NullableTypeConverter<TInterface> : JsonConverter<TInterface>
	where TInterface : class
{
	public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		reader.GetString()?.ToType()?.ToInstance<TInterface>();

	public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value.GetType().AssemblyQualifiedName);
}
