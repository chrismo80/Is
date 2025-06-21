using System.Diagnostics;

namespace Is.Core;

[DebuggerStepThrough]
public static class Check
{
	public static Result<T> That<T>(T value, Func<T, bool> predicate) =>
		new(predicate(value), value);

	public static Failure<bool> That(bool condition) =>
		new(condition, true);
}

[DebuggerStepThrough]
public readonly struct Result<T>(bool condition, T value)
{
	public Failure<TResult> Yields<TResult>(Func<T, TResult> result) => condition switch
	{
		true => new Failure<TResult>(true, result(value)),
		false => new Failure<TResult>(false, default)
	};
}

[DebuggerStepThrough]
public readonly struct Failure<TResult>(bool condition, TResult result)
{
	public TResult Unless(object? actual, string message, object? other) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message, other)
	};

	public TResult Unless(object? actual, string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message),
	};

	public TResult Unless(string message, List<string> text) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(message, text),
	};
}