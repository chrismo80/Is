using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Return
{
	public static Conditional<T> When<T>(T value, Func<T, bool> predicate) =>
		new(predicate(value), value);

	public static Failable<bool> Check(bool condition) =>
		new(condition, true);
}

[DebuggerStepThrough]
public readonly struct Conditional<T>(bool condition, T value)
{
	public Failable<TResult> Then<TResult>(Func<T, TResult> result) => condition switch
	{
		true => new Failable<TResult>(true, result(value)),
		false => new Failable<TResult>(false, default)
	};
}

[DebuggerStepThrough]
public readonly struct Failable<TResult>(bool condition, TResult result)
{
	public TResult Otherwise(object? actual, string message, object? other) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message, other)
	};

	public TResult Otherwise(object? actual, string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message)
	};
}