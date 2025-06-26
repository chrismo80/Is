using Is.Core;

namespace Is.Tests;

public class NUnitTestAdapter : ITestAdapter
{
	public void ReportSuccess()
	{ }

	public void ReportFailure(NotException ex) =>
		throw new AssertionException(ex.Message, ex);

	public void ReportFailures(string message, List<NotException> failures)
	{
		var messages = string.Join("\n", failures.Select(f => f.Message));

		throw new AssertionException($"{message}\n{messages}");
	}
}