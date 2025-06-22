using System.Diagnostics;

namespace Is.Core;

[DebuggerStepThrough]
public static class Check
{
	public static Failure<bool> That(bool condition) =>
		new(condition, true);

	public static Result<TValue> That<TValue>(TValue value, Func<TValue, bool> predicate) =>
		new(predicate(value), value);
}

[DebuggerStepThrough]
public readonly struct Result<TValue>(bool condition, TValue value)
{
	public Failure<TResult> Yields<TResult>(Func<TValue, TResult> result) => condition switch
	{
		true => new Failure<TResult>(true, result(value)),
		false => new Failure<TResult>(false, default)
	};
}

[DebuggerStepThrough]
public readonly struct Failure<TResult>(bool condition, TResult? result)
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
}