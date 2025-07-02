using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.Core.FailureObservers;

[DebuggerStepThrough]
public class JsonObserver : IFailureObserver
{
	const string FILE = "FailureReport.json";

	private static readonly List<Failure> failures = [];
	private static readonly object sync = new();

	public void OnFailure(Failure failure)
	{
		lock (sync)
		{
			failures.Add(failure);
			failures.SaveJson(FILE);
		}
	}
}