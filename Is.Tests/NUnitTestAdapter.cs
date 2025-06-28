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
		var messages = string.Join("\n\n", failures.Select(f => f.Message));

		throw new AssertionException($"{message}\n{messages}");
	}
}