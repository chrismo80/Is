using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Check
{
	public static Result<T> That<T>(T value, Func<T, bool> predicate) =>
		new(predicate(value), value);

	public static Failure<bool> That(bool condition) =>
		new(condition, true, null);

	public static Failure<bool> That<T>(T value, Func<T, bool> predicate, Func<T, object?>? failureContextExtractor) =>
		new (predicate(value), true, failureContextExtractor?.Invoke(value));
}

[DebuggerStepThrough]
public readonly struct Result<T>(bool condition, T value)
{
	public Failure<TResult> Yields<TResult>(Func<T, TResult> result) => condition switch
	{
		true => new Failure<TResult>(true, result(value), null),
		false => new Failure<TResult>(false, default, null)
	};
}

[DebuggerStepThrough]
public readonly struct Failure<TResult>(bool condition, TResult result, object? failureContext)
{
	public TResult Unless(object? actual, string message, object? other) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message, other)
	};

	public TResult Unless(object? actual, string message) => condition switch
	{
		true => Assertion.Passed(result),
		false when failureContext == null => Assertion.Failed<TResult>(actual, message),
		false => Unless(actual, message, failureContext),
	};

	public TResult Unless(string message) => condition switch
	{
		true => Assertion.Passed(result),
		false when failureContext is List<string> text => Assertion.Failed<TResult>(message, text),
		false => Assertion.Failed<TResult>(message),
	};
}