using Is.Core;
using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IAssertionObserver"/> that writes all failed assertions into
/// one FailureReport JSON file.
/// </summary>
[DebuggerStepThrough]
public class JsonObserver : IAssertionObserver
{
	private static readonly List<AssertionEvent> failedEvents = [];
	private static readonly object sync = new();

	public string Filename { get; set; } = "FailureReport.json";

	public void OnAssertion(AssertionEvent assertionEvent)
	{
		if (assertionEvent.Passed)
			return;

		lock (sync)
		{
			failedEvents.Add(assertionEvent);
			failedEvents.SaveJson(Filename);
		}
	}
}