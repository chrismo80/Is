using System.Diagnostics;

namespace Is.Core;

[DebuggerStepThrough]
public static class Check
{
	public static Failable<bool> That(bool condition) =>
		new(condition, true);

	public static Returnable<TValue> That<TValue>(TValue value, Func<TValue, bool> predicate) =>
		new(predicate(value), value);
}

[DebuggerStepThrough]
public readonly struct Returnable<TValue>(bool condition, TValue value)
{
	public Failable<TResult> Yields<TResult>(Func<TValue, TResult> result) => condition switch
	{
		true => new Failable<TResult>(true, result(value)),
		false => new Failable<TResult>(false, default)
	};
}

[DebuggerStepThrough]
public readonly struct Failable<TResult>(bool condition, TResult? result)
{
	public TResult? Unless(object? actual, string message, object? other) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message, other)
	};

	public TResult? Unless(object? actual, string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message),
	};

	public TResult? Unless(string message, List<string> text) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(message, text),
	};

	public TResult? Unless(string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(message),
	};
}