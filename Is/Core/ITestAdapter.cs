using Is.Tools;

namespace Is.Core;

/// <summary>
/// Represents an interface for handling test result reporting.
/// Serves as a hook for custom test frameworks to throw custom exception types.
/// Can be set via Configuration.TestAdapter.
/// </summary>
public interface ITestAdapter
{
	/// <summary>Reports a successful test result to the configured test adapter.</summary>
	void ReportSuccess();

	/// <summary>Reports a failed test result to the configured test adapter.</summary>
	void ReportFailure(Failure failure);

	/// <summary>Reports multiple test failures to the configured test adapter.</summary>
	void ReportFailures(string message, List<Failure> failures);
}

/// <summary>
/// Provides a default implementation of the ITestAdapter interface.
/// Throws exceptions for test failures, specifically NotException for single failures and AggregateException for multiple failures.
/// </summary>
public class DefaultTestAdapter : ITestAdapter
{
	public void ReportSuccess() { }

	public void ReportFailure(Failure failure) =>
		throw new NotException(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(f => new NotException(f)));
}

/// <summary>
/// A test adapter implementation that suppresses specific output for test assertions.
/// Provides minimal reporting behaviour by silencing failure messages.
/// Serves as a silent alternative to more verbose test adapters.
/// </summary>
public class SilentTestAdapter : ITestAdapter
{
	public void ReportSuccess() { }

	public void ReportFailure(Failure failure) =>
		Console.WriteLine(failure.Message.RemoveLineBreaks());

	public void ReportFailures(string message, List<Failure> failures)
	{
		foreach (var failure in failures)
			ReportFailure(failure);
	}
}