using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public class NotException(Failure failure) : Exception(failure.Message)
{
	/// <summary>
	/// The failure that caused the assertion to fail.
	/// </summary>
	public Failure Failure => failure;
}