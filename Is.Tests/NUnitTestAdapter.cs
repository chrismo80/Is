using Is.Core;
using Is.Tools;

namespace Is.Tests;

public class NUnitTestAdapter : ITestAdapter
{
	public void ReportSuccess() { }

	public void ReportFailure(Failure failure) =>
		throw new AssertionException(failure.Message);

	public void ReportFailures(string message, List<Failure> failures)
	{
		var messages = string.Join("", failures.Select(f => f.Message.RemoveLineBreaks()));

		throw new AssertionException($"{message}\n{messages}");
	}
}