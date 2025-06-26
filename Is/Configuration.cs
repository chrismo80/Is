using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Is;

/// <summary>
/// Global configurations that control assertion behaviour
/// </summary>
[DebuggerStepThrough]
public class Configuration
{
	public static Configuration Default { get; } = new();

	/// <summary>
	/// Controls whether assertion failures should throw a <see cref="NotException"/>.
	/// Default is true. If not set, assertions will return false on failure and log the message.
	/// </summary>
	public bool ThrowOnFailure { get; set; } = true;

	/// <summary>
	/// A logger delegate to use when <see cref="ThrowOnFailure"/> is false.
	/// Default case, messages will be written to <c>Debug.WriteLine</c>.
	/// </summary>
	[JsonIgnore]
	public Action<string?>? Logger { get; set; } = msg => Debug.WriteLine(msg);

	/// <summary>
	/// Makes code line info in <see cref="NotException"/> optional.
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
}