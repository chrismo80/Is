using Is.Tools;

namespace Is.Core.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that simply logs the failures to the Console, does not throw any exceptions
/// </summary>
public class ConsoleAdapter : ITestAdapter
{
	public void ReportFailure(Failure failure) =>
		Console.WriteLine(failure.Message.RemoveLineBreaks());

	public void ReportFailures(string message, List<Failure> failures) =>
		failures.ForEach(ReportFailure);
}