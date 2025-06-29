using Is.Tools;

namespace Is.Core.Interfaces;

/// <summary>
/// Interface providing a mechanism to observe and handle failure events.
/// Implementors of this interface are responsible for defining the logic
/// on how to handle or log failure details for diagnostic or reporting purposes.
/// </summary>
public interface IFailureObserver
{
	void OnFailure(Failure failure);
}

public class MarkDownObserver : IFailureObserver
{
	const string FILE = "FailureReport.md";

	static readonly object sync = new();

	static MarkDownObserver() => File.Delete(FILE);

	public void OnFailure(Failure failure)
	{
		var markdown = failure.ToMarkDown();

		lock (sync)
		{
			File.AppendAllText(FILE, markdown);
		}
	}
}