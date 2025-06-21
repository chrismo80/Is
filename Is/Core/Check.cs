using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Check
{
	public static ThatActual<TResult> That<TResult>(Func<TResult> function) =>
		new(function());
}

[DebuggerStepThrough]
public class ThatActual<TResult>(TResult? result)
{
	public TResult OrFailWith(object? actual, string fails)
	{
		if (EqualityComparer<TResult>.Default.Equals(result, default))
			return Assertion.Failed<TResult>(actual, fails);

		return Assertion.Passed(result);
	}

	public TResult OrFailWith(object? actual, string fails, object? other)
	{
		if (EqualityComparer<TResult>.Default.Equals(result, default))
			return Assertion.Failed<TResult>(actual, fails, other);

		return Assertion.Passed(result);
	}
}