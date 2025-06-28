using System.Runtime.CompilerServices;

namespace Is.Core;

/// <summary>
/// Represents a scoped context that captures all assertion failures within its lifetime and
/// reports the collection upon disposal if any failures occurred.
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
/// context.Failed.Is(2);
/// context.NextFailure().Message.IsContaining("false.IsTrue()");
///
/// context.Failed.Is(1);
/// context.NextFailure().Message.IsContaining("4.Is(5)");
/// </code>
///
/// If any failures are not dequeued or handled manually, they will be reported when the context is disposed.
/// </remarks>
public sealed class AssertionContext : IDisposable
{
	private static readonly AsyncLocal<AssertionContext?> current = new();

	private readonly Queue<Failure> _failures = [];

	private string? _caller;

	/// <summary>The number of remaining assertion failures in the context.</summary>
	public int Failed => _failures.Count;

	/// <summary>The number of passed assertions in the context.</summary>
	public int Passed { get; private set; }

	/// <summary>The total number of assertions in the context.</summary>
	public int Total => Passed + Failed;

	/// <summary>The ratio of passed assertions.</summary>
	public double Ratio => Total > 0 ? (double)Passed / Total : 1;

	/// <summary>Local configuration settings (copy of global <see cref="Configuration"/>) only active during the <see cref="AssertionContext"/>.</summary>
	internal Configuration Configuration { get; } = Configuration.Default.Clone();

	/// <summary>
	/// The current active <see cref="AssertionContext"/> for the asynchronous operation, or null if no context is active.
	/// </summary>
	public static AssertionContext? Current => current.Value;

	internal static bool IsActive => current.Value is not null;

	private AssertionContext()
	{ }

	/// <summary>
	/// Starts a new <see cref="AssertionContext"/> on the current thread.
	/// Current <see cref="Configuration"/> is cloned to enable local configurations per assertion context.
	/// </summary>
	public static AssertionContext Begin([CallerMemberName] string? method = null)
	{
		if (IsActive)
			throw new InvalidOperationException("AssertionContext already active on this async context.");

		current.Value = new AssertionContext { _caller = method };

		return current.Value;
	}

	/// <summary>
	/// Ends the assertion context and reports all collected failures to the <see cref="ITestAdapter"/>
	/// </summary>
	public void Dispose()
	{
		var testAdapter = Configuration.Active.TestAdapter;

		current.Value = null;

		if (_failures.Count == 0)
			return;

		var s = Total == 1 ? "" : "s";

		var message = $"{_failures.Count} of {Total} assertion{s} ({Ratio:P1}) failed in '{_caller}'";

		testAdapter.ReportFailures(message, _failures.ToList());
	}

	/// <summary>
	/// Dequeues an <see cref="Failure"/> from the queue to not be reported at the end of the context.
	/// </summary>
	public Failure NextFailure() => _failures.Dequeue();

	/// <summary>
	/// Dequeues as many <see cref="Failure"/>s specified in <paramref name="count"/> from the queue.
	/// </summary>
	public List<Failure> TakeFailures(int count) => Take(count).ToList();

	internal void AddFailure(Failure failure) => _failures.Enqueue(failure);

	internal void AddSuccess() => Passed++;

	private IEnumerable<Failure> Take(int count)
	{
		while(count-- > 0)
			yield return _failures.Dequeue();
	}
}