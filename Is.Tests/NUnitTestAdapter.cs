using Is.Core;
using Is.Tools;

namespace Is.Tests;

public class NUnitTestAdapter : ITestAdapter
{
	public void ReportSuccess() { }

	public void ReportFailure(NotException ex) =>
		throw new AssertionException(ex.Message, ex);

	public void ReportFailures(string message, List<NotException> failures)
	{
		var messages = string.Join("", failures.Select(f => f.Message.RemoveLineBreaks()));

		throw new AssertionException($"{message}\n{messages}");
	}
}