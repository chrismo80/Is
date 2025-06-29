using System.Diagnostics;
using Is.Tools;

namespace Is.Core;

/// <summary>
/// Offers a fluent API to assert conditions and create return values and error messages.
/// Can be used for custom assertions
/// </summary>
[DebuggerStepThrough]
public static class Check
{
	/// <summary>
	/// Evaluates a boolean condition.
	/// </summary>
	public static Failable<bool> That(bool condition) =>
		new(condition, true);

	public static Returnable<TValue> That<TValue>(TValue value, Func<TValue, bool> predicate) =>
		new(predicate(value), value);
}

[DebuggerStepThrough]
public readonly struct Returnable<TValue>(bool condition, TValue value)
{
	/// <summary>
	/// Projects a result from the original value if the initial predicate condition was true.
	/// </summary>
	public Failable<TResult> Yields<TResult>(Func<TValue, TResult> result) => condition switch
	{
		true => new Failable<TResult>(true, result(value)),
		false => new Failable<TResult>(false, default)
	};
}

[DebuggerStepThrough]
public readonly struct Failable<TResult>(bool condition, TResult? result)
{
	/// <summary>
	/// Returns the result if the condition is true; otherwise, triggers a failure with a message.
	/// </summary>
	public TResult? Unless(object? actual, string message, object? other) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(actual, message, other)
	};

	public TResult? Unless(object? actual, string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(new Failure(actual.Actually(message), actual)),
	};

	public TResult? Unless(string message) => condition switch
	{
		true => Assertion.Passed(result),
		false => Assertion.Failed<TResult>(new Failure(message)),
	};
}