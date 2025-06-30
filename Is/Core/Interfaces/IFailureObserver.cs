using Is.Tools;

namespace Is.Core.Interfaces;

/// <summary>
/// Interface providing a mechanism to observe and handle failure events.
/// Implementors of this interface are responsible for defining the logic
/// on how to handle or log failure details for diagnostic or reporting purposes.
/// </summary>
public interface IFailureObserver
{
	/// <summary>
	/// This method is invoked when a failure occurs during an assertion.
	/// Observer can perform custom logic on that failure such as logging or reporting.
	/// </summary>
	void OnFailure(Failure failure);
}

public class MarkDownObserver : IFailureObserver
{
	const string FILE = "FailureReport.md";

	static readonly object sync = new();

	static MarkDownObserver() => File.Delete(FILE);

	public void OnFailure(Failure failure) => Append(failure.ToMarkDown());

	private static void Append(string text)
	{
		lock (sync)
		{
			File.AppendAllText(FILE, text);
		}
	}
}