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
	void ReportFailure(NotException ex);

	/// <summary>Reports multiple test failures to the configured test adapter.</summary>
	void ReportFailures(string message, List<NotException> failures);
}

public class TestAdapter : ITestAdapter
{
	public void ReportSuccess()
	{ }

	public void ReportFailure(NotException ex) =>
		throw ex;

	public void ReportFailures(string message, List<NotException> failures) =>
		throw new AggregateException(message, failures);
}