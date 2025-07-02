using Is.Core;
using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IFailureObserver"/> that writes all failures into
/// one FailureReport JSON file.
/// </summary>
[DebuggerStepThrough]
public class JsonObserver : IFailureObserver
{
	private static readonly List<Failure> failures = [];
	private static readonly object sync = new();

	public string Filename { get; set; } = "FailureReport.json";

	public void OnFailure(Failure failure)
	{
		lock (sync)
		{
			failures.Add(failure);
			failures.SaveJson(Filename);
		}
	}
}