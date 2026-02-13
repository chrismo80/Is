using System.Diagnostics;
using System.Reflection;
using Is.Tools;

namespace Is.Core;

[DebuggerStepThrough]
internal static class Assertion
{
	private static readonly Assembly Mine = Assembly.GetExecutingAssembly();

	internal static bool Passed() => Passed(true);

	internal static T Passed<T>(T result)
	{
		Configuration.Active.AssertionListener?.OnAssertion(CreatePassedEvent());

		return result;
	}

	internal static T? Failed<T>(Failure failure)
	{
		Configuration.Active.AssertionListener?.OnAssertion(new AssertionEvent(false, failure));

		Configuration.Active.FailureObserver?.OnFailure(failure);

		if (AssertionContext.IsActive)
			AssertionContext.Current?.AddFailure(failure);
		else
			Configuration.Active.TestAdapter?.ReportFailure(failure);

		return default;
	}

	internal static T? Failed<T>(object? actual, string equality, object? expected) =>
		Failed<T>(new Failure(actual.Actually(equality, expected), actual, expected));

	internal static T? Failed<T>(object? actual, string equality) =>
		Failed<T>(new Failure(actual.Actually(equality), actual));

	private static AssertionEvent CreatePassedEvent()
	{
		var frames = new StackTrace(true).GetFrames();

		var assertionFrame = frames.FirstOrDefault(f =>
			f.GetMethod()?.DeclaringType?.Assembly == Mine &&
			f.GetMethod()?.DeclaringType != typeof(Assertion) &&
			f.GetMethod()?.DeclaringType != typeof(Check));

		var codeFrame = frames.FirstOrDefault(f =>
			f.GetMethod()?.DeclaringType?.Assembly != Mine &&
			f.GetFileName() != null);

		return new AssertionEvent(
			true,
			assertionFrame?.GetMethod()?.Name,
			codeFrame?.GetFileName(),
			codeFrame?.GetFileLineNumber());
	}
}
