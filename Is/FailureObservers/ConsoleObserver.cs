using Is.Core;
using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IAssertionObserver"/> that writes all failed assertions directly to the Console.
/// </summary>
[DebuggerStepThrough]
public class ConsoleObserver : IAssertionObserver
{
	public void OnAssertion(AssertionEvent assertionEvent)
	{
		if (!assertionEvent.Passed)
			Console.WriteLine(assertionEvent.Message?.RemoveLineBreaks());
	}
}