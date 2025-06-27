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

public class TestAdapter : ITestAdapter
{
	public void ReportSuccess() { }

	public void ReportFailure(Failure failure) =>
		throw new NotException(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(f => new NotException(f)));
}