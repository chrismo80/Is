using Is.Core;
using Is.Core.Interfaces;

namespace Is.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that allows throwing a custom exception type
/// <typeparamref name="TException"/> when test failures are reported.
/// </summary>
public class CustomExceptionAdapter<TException>(Func<AssertionEvent, TException> exceptionFactory) : ITestAdapter
	where TException : Exception
{
	public void ReportFailure(AssertionEvent assertionEvent) =>
		throw exceptionFactory(assertionEvent);

	public void ReportFailures(string message, List<AssertionEvent> assertionEvents) =>
		throw new AggregateException(message, assertionEvents.Select(exceptionFactory));
}