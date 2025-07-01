using Is.Tools;
using Is.Core.Interfaces;

namespace Is.Core.FailureObservers;

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