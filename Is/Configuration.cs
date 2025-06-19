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
	public static Action<string?>? Logger { get; set; } = msg => System.Diagnostics.Debug.WriteLine(msg);

	internal static T? Throw<T>(this NotException ex)
	{
		if (ThrowOnFailure)
			throw ex;

		Logger?.Invoke(ex.Message);

		return default;
	}

	internal static bool Throw(this NotException ex) => ex.Throw<bool>();
}