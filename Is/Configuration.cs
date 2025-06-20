using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Configuration
{
	/// <summary>
	/// Gets or sets a value indicating whether assertion failures should throw a <see cref="NotException"/>.
	/// Default is true. If set to false, assertions will return false on failure and log the message.
	/// </summary>
	public static bool ThrowOnFailure { get; set; } = true;

	/// <summary>
	/// Gets or sets the logger delegate to use when <see cref="ThrowOnFailure"/> is false.
	/// Default case, messages will be written to <c>Debug.WriteLine</c>.
	/// </summary>
	public static Action<string?>? Logger { get; set; } = msg => Debug.WriteLine(msg);

	/// <summary>
	/// Default value used for floating point comparisons if not specified specifically
	/// </summary>
	public static double FloatingPointComparisonFactor { get; set; } = 1e-6;

	/// <summary>
	/// Makes code line info in <see cref="NotException"/> optional
	/// </summary>
	public static bool AppendCodeLine { get; set; } = true;

	internal static T? HandleFailure<T>(this NotException ex)
	{
		if (ThrowOnFailure && !AssertionContext.IsActive)
			throw ex;

		AssertionContext.Current?.AddFailure(ex);

		Logger?.Invoke(ex.Message);

		return default;
	}
}