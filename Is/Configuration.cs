using Is.Core;
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
	internal static Configuration Default { get; } = new();

	public static Configuration Active => AssertionContext.Current?.Configuration ?? Default;

	/// <summary>
	/// Specifies the adapter responsible for handling assertion results.
	/// The adapter decides whether the failure should result in a thrown exception,
	/// the type of this exception or if the failure should be silently handled
	/// via simple logging or data export for further failure analysis
	/// Default is throwing <see cref="NotException"/>.
	/// </summary>
	public ITestAdapter TestAdapter { get; set; } = new DefaultTestAdapter();

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
	public JsonSerializerOptions JsonSerializerOptions { get; set; } = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
	};

	internal Configuration Clone() => new()
	{
		TestAdapter = TestAdapter,
		AppendCodeLine = AppendCodeLine,
		ColorizeMessages = ColorizeMessages,
		FloatingPointComparisonPrecision = FloatingPointComparisonPrecision,
		MaxRecursionDepth = MaxRecursionDepth,
		ParsingFlags = ParsingFlags,
		JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions),
	};
}