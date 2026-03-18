using Is.Core;
using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.AssertionObservers;

/// <summary>
/// <see cref="IAssertionObserver"/> that writes all failed assertions into
/// one FailureReport JSON file.
/// </summary>
[DebuggerStepThrough]
public class JsonObserver : IAssertionObserver
{
	private const string FILENAME = "FailureReport.json";
	
	private static readonly List<AssertionEvent> failedEvents = [];
	private static readonly object sync = new();

	static JsonObserver() => Reset();

	public void OnAssertion(AssertionEvent assertionEvent)
	{
		if (assertionEvent.Passed)
			return;

		lock (sync)
		{
			failedEvents.Add(assertionEvent);
			failedEvents.SaveJson(FILENAME);
		}
	}
	
	public static void Reset()
	{
		lock (sync)
		{
			File.Delete(FILENAME);
		}
	}
}