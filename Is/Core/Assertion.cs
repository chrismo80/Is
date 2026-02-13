using System.Diagnostics;
using System.Reflection;
using Is.Tools;

namespace Is.Core;

[DebuggerStepThrough]
internal static class Assertion
{
	internal static bool Passed() => Passed(true);

	internal static T Passed<T>(T result)
	{
		var assertionEvent = CreatePassedEvent();
		Configuration.Active.AssertionObserver?.OnAssertion(assertionEvent);
		return result;
	}

	internal static T? Failed<T>(string message, object? actual = null, object? expected = null, 
		Type? customExceptionType = null, List<AssertionEvent>? innerEvents = null)
	{
		var assertionEvent = CreateFailedEvent(message, actual, expected, customExceptionType, innerEvents);
		
		Configuration.Active.AssertionObserver?.OnAssertion(assertionEvent);

		if (AssertionContext.IsActive)
			AssertionContext.Current?.AddFailure(assertionEvent);
		else
			Configuration.Active.TestAdapter?.ReportFailure(assertionEvent);

		return default;
	}

	private static AssertionEvent CreatePassedEvent()
	{
		var frames = new StackTrace(true).GetFrames();
		var codeFrame = frames.FindFrame();
		var assertionFrame = codeFrame != null ? frames[Array.IndexOf(frames, codeFrame) - 1] : null;

		var fileName = codeFrame?.GetFileName();
		var lineNumber = codeFrame?.GetFileLineNumber();

		return AssertionEvent.CreatePassed(
			assertionFrame?.GetMethod()?.Name,
			fileName,
			lineNumber,
			fileName != null && lineNumber.HasValue ? fileName.GetLine(lineNumber.Value) : null);
	}

	private static AssertionEvent CreateFailedEvent(string message, object? actual, object? expected,
		Type? customExceptionType = null, List<AssertionEvent>? innerEvents = null)
	{
		var frames = new StackTrace(true).GetFrames();
		var codeFrame = frames.FindFrame();
		var assertionFrame = codeFrame != null ? frames[Array.IndexOf(frames, codeFrame) - 1] : null;

		var fileName = codeFrame?.GetFileName();
		var lineNumber = codeFrame?.GetFileLineNumber();
		var code = fileName != null && lineNumber.HasValue ? fileName.GetLine(lineNumber.Value) : null;

		return new AssertionEvent(
			false,
			message.AppendCodeLine(codeFrame),
			actual,
			expected,
			assertionFrame?.GetMethod()?.Name,
			codeFrame?.GetMethod()?.Name,
			fileName,
			lineNumber,
			code,
			customExceptionType,
			innerEvents);
	}
}
