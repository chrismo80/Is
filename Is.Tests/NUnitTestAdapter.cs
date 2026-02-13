using Is.Core;
using Is.Core.Interfaces;

namespace Is.Tests;

public class NUnitTestAdapter : ITestAdapter
{
	public void ReportFailure(AssertionEvent assertionEvent) =>
		throw new AssertionException(assertionEvent.Message ?? "Assertion failed");

	public void ReportFailures(string message, List<AssertionEvent> assertionEvents)
	{
		var messages = string.Join("\n\n", assertionEvents.Select(a => a.Message));

		throw new AssertionException($"{message}\n{messages}");
	}
}