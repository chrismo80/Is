using Is.Core;
using Is.Core.Interfaces;

namespace Is.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that allows throwing a custom exception type
/// <typeparamref name="TException"/> when test failures are reported.
/// </summary>
public class CustomExceptionAdapter<TException>(Func<Failure, TException> exceptionFactory) : ITestAdapter
	where TException : Exception
{
	public void ReportFailure(Failure failure) =>
		throw exceptionFactory(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(exceptionFactory));
}