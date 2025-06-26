namespace Is.Core;

/// <summary>
/// Represents an interface for handling test result reporting.
/// Can be set via Configuration.TestAdapter
/// </summary>
public interface ITestAdapter
{
	void ReportSuccess();

	void ReportFailure(NotException ex);

	void ReportFailures(string message, List<NotException> messages);
}

public class DefaultTestAdapter : ITestAdapter
{
	public void ReportSuccess() {}

	public void ReportFailure(NotException ex) =>
		throw ex;

	public void ReportFailures(string message, List<NotException> failures) =>
		throw new AggregateException(message, failures);
}