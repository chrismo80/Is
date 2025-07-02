namespace Is.Core.Interfaces;

/// <summary>
/// Interface providing a mechanism to observe failures.
/// Can be set via Configuration.FailureObserver.
/// </summary>
public interface IFailureObserver
{
	/// <summary>
	/// This method is invoked when a failure occurs during an assertion.
	/// Observer can perform custom logic on that failure such as logging or reporting.
	/// </summary>
	void OnFailure(Failure failure);
}