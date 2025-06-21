using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Check
{
	public static Conditional<T> When<T>(T value, Func<T, bool> predicate) =>
		new(predicate(value), value);

	public static Failable<bool> Return(bool condition) =>
		new(condition, true);
}

[DebuggerStepThrough]
public readonly struct Conditional<T>(bool condition, T value)
{
	public Failable<TResult> Return<TResult>(Func<T, TResult> result) => condition ?
		new Failable<TResult>(true, result(value)) :
		new Failable<TResult>(false, default);
}

[DebuggerStepThrough]
public readonly struct Failable<TResult>(bool condition, TResult result)
{
	public TResult OrFail(object? actual, string message, object? other)
	{
		if (!condition)
			return Assertion.Failed<TResult>(actual, message, other);

		return Assertion.Passed(result);
	}

	public TResult OrFail(object? actual, string message)
	{
		if (!condition)
			return Assertion.Failed<TResult>(actual, message);

		return Assertion.Passed(result);
	}
}