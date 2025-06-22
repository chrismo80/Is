using System.Diagnostics;
using System.Reflection;

namespace Is.Core;

[DebuggerStepThrough]
public static class Configuration
{
	/// <summary>
	/// Controls whether assertion failures should throw a <see cref="NotException"/>.
	/// Default is true. If set to false, assertions will return false on failure and log the message.
	/// </summary>
	public static bool ThrowOnFailure { get; set; } = true;

	/// <summary>
	/// A logger delegate to use when <see cref="ThrowOnFailure"/> is false.
	/// Default case, messages will be written to <c>Debug.WriteLine</c>.
	/// </summary>
	public static Action<string?>? Logger { get; set; } = msg => Debug.WriteLine(msg);

	/// <summary>
	/// Comparison factor used for floating point comparisons if not specified specifically
	/// Default value is 1e-6
	/// </summary>
	public static double FloatingPointComparisonFactor { get; set; } = 1e-6;

	/// <summary>
	/// Makes code line info in <see cref="NotException"/> optional
	/// </summary>
	public static bool AppendCodeLine { get; set; } = true;

	/// <summary>
	/// Controls the maximum depth of recursion when parsing deeply nested objects
	/// Default value is 20
	/// </summary>
	public static int MaxRecursionDepth { get; set; } = 20;

	/// <summary>
	/// Controls the binding flags to use when parsing deeply nested objects
	/// Default is public | non-public | instance
	/// </summary>
	public static BindingFlags ParsingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
}