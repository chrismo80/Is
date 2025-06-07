using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Performance
{
	/// <summary>
	/// Asserts that the given <paramref name="action" /> did complete
	/// within a specific <paramref name="timeout" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Action action, TimeSpan timeout)
	{
		if (Task.Run(action).Wait(timeout))
			return true;

		throw new NotException(action, "was not completing within", timeout);
	}

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> did complete
	/// within a specific <paramref name="timeout" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Func<Task> function, TimeSpan timeout)
	{
		var action = () => function().GetAwaiter().GetResult();

		return action.IsCompletingWithin(timeout);
	}
}