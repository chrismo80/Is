using Is.Tools;

namespace Is.Core.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that creates a failure report, does not throw any exceptions
/// </summary>
public class MarkDownAdapter : ITestAdapter
{
	private readonly string _file;

	public MarkDownAdapter(string fileName  = "FailureReport.md")
	{
		_file = fileName;

		File.Delete(_file);
	}

	public void ReportFailure(Failure failure) =>
		File.AppendAllText(_file, failure.ToMarkDown());

	public void ReportFailures(string message, List<Failure> failures) =>
		failures.ForEach(ReportFailure);
}