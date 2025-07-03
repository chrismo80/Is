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
/// Global configurations that control assertion behaviour
/// </summary>
[DebuggerStepThrough]
public class Configuration
{
	const string ConfigFile = "is.configuration.json";
	internal static Configuration Default { get; } = ConfigFile.LoadJson<Configuration>() ?? new Configuration();

	public static Configuration Active => AssertionContext.Current?.Configuration ?? Default;

	/// <summary>
	/// Determines the observer responsible for handling failure events during assertions.
	/// The observer implements logic for capturing, processing, or reporting failures,
	/// enabling customisation of diagnostic or reporting mechanisms.
	/// Default is <see cref="MarkDownObserver"/>.
	/// </summary>
	[JsonConverter(typeof(TypeConverter<IFailureObserver, MarkDownObserver>))]
	public IFailureObserver? FailureObserver { get; set; } = new MarkDownObserver();

	/// <summary>
	/// Specifies the adapter responsible for integrating the assertion framework with external testing frameworks.
	/// Default is <see cref="DefaultAdapter"/>that is throwing <see cref="NotException"/> for single failures
	/// and a <see cref="AggregateException"/> for multiple failures.
	/// </summary>
	[JsonConverter(typeof(TypeConverter<ITestAdapter, DefaultAdapter>))]
	public ITestAdapter? TestAdapter { get; set; } = new DefaultAdapter();

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
		FailureObserver = FailureObserver,
		TestAdapter = TestAdapter,
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