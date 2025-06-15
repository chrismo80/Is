using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Performance
{
	/// <summary>
	/// Asserts that the given <paramref name="action" /> did complete
	/// within a specific <paramref name="timespan" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Action action, TimeSpan timespan)
	{
		if (Task.Run(action).Wait(timespan))
			return true;

		throw new NotException(action, "was not completing within", timespan);
	}

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> did complete
	/// within a specific <paramref name="timespan" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Func<Task> function, TimeSpan timespan) =>
		function.ToAction().IsCompletingWithin(timespan);
}