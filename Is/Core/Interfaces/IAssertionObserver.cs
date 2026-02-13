namespace Is.Core.Interfaces;

/// <summary>
/// Interface providing a mechanism to observe assertion outcomes.
/// Can be set via Configuration.AssertionObserver.
/// Replaces IFailureObserver - receives all assertions (passed and failed).
/// </summary>
public interface IAssertionObserver
{
	/// <summary>
	/// This method is invoked for every assertion evaluation (passed and failed).
	/// Observer can perform custom logic such as logging or reporting.
	/// </summary>
	void OnAssertion(AssertionEvent assertionEvent);
}