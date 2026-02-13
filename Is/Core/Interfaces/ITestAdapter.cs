namespace Is.Core.Interfaces;

/// <summary>
/// Represents an interface for handling test result reporting.
/// Serves as a hook for custom test frameworks to throw custom exception types.
/// Can be set via Configuration.TestAdapter.
/// </summary>
public interface ITestAdapter
{
	/// <summary>Reports a failed assertion to the configured test adapter.</summary>
	void ReportFailure(AssertionEvent assertionEvent);

	/// <summary>Reports multiple failed assertions to the configured test adapter.</summary>
	void ReportFailures(string message, List<AssertionEvent> assertionEvents);
}