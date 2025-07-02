using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.Core.FailureObservers;

/// <summary>
/// <see cref="IFailureObserver"/> that writes all failures directly to the Console.
/// </summary>
[DebuggerStepThrough]
public class ConsoleObserver : IFailureObserver
{
	public void OnFailure(Failure failure) =>
		Console.WriteLine(failure.Message.RemoveLineBreaks());
}