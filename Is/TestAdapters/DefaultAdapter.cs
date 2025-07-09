using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;

namespace Is.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that is throwing <see cref="NotException"/>s for
/// single failures and an <see cref="AggregateException"/> for multiple failures.
/// </summary>
[DebuggerStepThrough]
public class DefaultAdapter : ITestAdapter
{
	public void ReportFailure(Failure failure) =>
		throw failure.ToException();

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(f => f.ToException()));
}