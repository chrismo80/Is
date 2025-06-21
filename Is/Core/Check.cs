using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Check
{
	/// <summary>
	/// Starts a fluent verification chain by specifying the actual value to check.
	/// </summary>
	public static ThatActual<TActual> That<TActual>(TActual? actual) =>
		new(actual);
}

[DebuggerStepThrough]
public class ThatActual<TActual>(TActual? actual)
{
	public ReturnsResult<TActual, TResult> Returns<TResult>(Func<TResult> function) =>
		new(actual, function);

	public AndOther<TActual, TOther> And<TOther>(TOther? other) =>
		new(actual, other);
}

[DebuggerStepThrough]
public class AndOther<TActual, TOther>(TActual? actual, TOther? other)
{
	public ReturnResult<TActual, TOther, TResult> Return<TResult>(Func<TResult> function) =>
		new(actual, other, function);
}

[DebuggerStepThrough]
public class ReturnsResult<TActual, TResult>(TActual? actual, Func<TResult> function)
{
	public TResult FailsIf(string fails)
	{
		var result = function();

		if (EqualityComparer<TResult>.Default.Equals(result, default))
			return Assertion.Failed<TResult>(actual, fails);

		return Assertion.Passed(result);
	}
}

[DebuggerStepThrough]
public class ReturnResult<TActual, TOther, TResult>(TActual? actual, TOther? other, Func<TResult> function)
{
	public TResult FailsIf(string fails)
	{
		var result = function();

		if (EqualityComparer<TResult>.Default.Equals(result, default))
			return Assertion.Failed<TResult>(actual, fails, other);

		return Assertion.Passed(result);
	}
}