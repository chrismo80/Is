using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;

namespace Is.AssertionListeners;

/// <summary>
/// <see cref="IAssertionListener"/> that logs every assertion outcome to the Console.
/// Useful for debugging and understanding assertion flow.
/// </summary>
[DebuggerStepThrough]
public class ConsoleAssertionListener : IAssertionListener
{
	public void OnAssertion(AssertionEvent e)
	{
		var status = e.Passed ? "PASS".Color(32) : "FAIL".Color(91);
		var name = e.Assertion ?? "?";
		var location = e.File is not null ? $" at {e.File}:{e.Line}" : "";

		Console.WriteLine($"[{status}] {name}{location}");
	}
}
