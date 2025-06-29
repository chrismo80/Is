namespace Is.Core.Interfaces;

/// <summary>
/// Represents an interface for handling test result reporting.
/// Serves as a hook for custom test frameworks to throw custom exception types.
/// Provides a default implementation that throws exceptions for test failures,
/// specifically a <see cref="NotException"/> for single failures
/// and a <see cref="AggregateException"/> for multiple failures.
/// Can be set via Configuration.TestAdapter.
/// </summary>
public interface ITestAdapter
{
	/// <summary>Reports a failed test result to the configured test adapter.</summary>
	void ReportFailure(Failure failure) =>
		throw new NotException(failure);

	/// <summary>Reports multiple test failures to the configured test adapter.</summary>
	void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(f => new NotException(f)));
}

/// <summary>
/// Default adapter using the default implementation of the <see cref="ITestAdapter"/> interface
/// </summary>
public class DefaultAdapter : ITestAdapter;