using System.Runtime.CompilerServices;

namespace Is.Core;

/// <summary>
/// Represents a scoped context that captures all assertion failures (as <see cref="NotException"/> instances)
/// within its lifetime and throws a single <see cref="AggregateException"/> upon disposal if any failures occurred.
/// </summary>
/// <remarks>
/// Intended for test frameworks like NUnit, xUnit, or MSTest. By using this context, tests can collect multiple
/// assertion failures and evaluate them together at the end of the test unless being dequeued before.
///
/// Usage:
/// <code>
/// using var context = AssertionContext.Begin();
///
/// false.IsTrue();
/// 4.Is(5);
///
/// context.FailureCount.Is(2);
/// context.NextFailure().Message.IsContaining("false.IsTrue()");
///
/// context.FailureCount.Is(1);
/// context.NextFailure().Message.IsContaining("4.Is(5)");
/// </code>
///
/// If any failures are not dequeued or handled manually, they will be thrown in an <see cref="AggregateException"/>
/// when the context is disposed.
/// </remarks>
public sealed class AssertionContext : IDisposable
{
	private static readonly AsyncLocal<AssertionContext?> current = new();

	private readonly Queue<NotException> _failures = [];

	private string? _caller;

	/// <summary>
	/// Gets the number of remaining assertion failures in the context.
	/// </summary>
	public int Failed => _failures.Count;

	public int Passed { get; private set; }

	public int Total => Passed + Failed;

	public double Ratio => (double)Passed / Total;

	public static AssertionContext? Current => current.Value;

	internal static bool IsActive => current.Value is not null;

	private AssertionContext()
	{ }

	/// <summary>
	/// Starts a new <see cref="AssertionContext"/> on the current thread.
	/// All assertion failures will be collected and thrown as an <see cref="AggregateException"/> when the context is disposed.
	/// </summary>
	public static AssertionContext Begin([CallerMemberName] string? method = null)
	{
		if (IsActive)
			throw new InvalidOperationException("AssertionContext already active on this async context.");

		current.Value = new AssertionContext() { _caller = method };

		return current.Value;
	}

	/// <summary>
	/// Ends the assertion context and validates all collected failures.
	/// If any assertions failed, throws an <see cref="AggregateException"/> containing all collected <see cref="Is.Core.NotException"/>s.
	/// </summary>
	public void Dispose()
	{
		current.Value = null;

		if (_failures.Count == 0)
			return;

		var s = _failures.Count == 1 ? "" : "s";

		throw new AggregateException($"{_failures.Count} of {Total} assertion{s} failed in '{_caller}'", _failures);
	}

	/// <summary>
	/// Dequeues an <see cref="Is.Core.NotException"/> from the queue to not be thrown at the end of the context.
	/// </summary>
	public NotException NextFailure() => _failures.Dequeue();

	internal void AddFailure(NotException ex) => _failures.Enqueue(ex);

	internal void AddSuccess() => Passed++;
}