using Is.Core;
using Is.Core.Interfaces;
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
	/// Determines the observer responsible for handling failure events during assertions.
	/// The observer implements logic for capturing, processing, or reporting failures,
	/// enabling customisation of diagnostic or reporting mechanisms.
	/// By default, failures are exported to a markdown FailureReport.
	/// </summary>
	public IFailureObserver FailureObserver { get; private init; } = new MarkDownObserver();

	/// <summary>
	/// Specifies the adapter responsible for integrating the assertion framework with external testing frameworks.
	/// By default, a <see cref="NotException"/>s are thrown.
	/// </summary>
	public ITestAdapter TestAdapter { get; set; } = new DefaultAdapter();

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