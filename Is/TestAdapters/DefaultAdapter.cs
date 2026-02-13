using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;

namespace Is.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that is throwing <see cref="NotException"/>s for
/// single failed assertions and an <see cref="AggregateException"/> for multiple failures.
/// </summary>
[DebuggerStepThrough]
public class DefaultAdapter : ITestAdapter
{
	public void ReportFailure(AssertionEvent assertionEvent) =>
		throw assertionEvent.ToException();

	public void ReportFailures(string message, List<AssertionEvent> assertionEvents) =>
		throw new AggregateException(message, assertionEvents.Select(a => a.ToException()));
}