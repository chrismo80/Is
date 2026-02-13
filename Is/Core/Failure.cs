using System.Diagnostics;
using System.Reflection;
using Is.Tools;

namespace Is.Core;

/// <summary>
/// Represents a failure encountered during an assertion or test execution.
/// Contains detailed information about the failure, including message, actual
/// and expected values, assertion details, and location in source code.
/// </summary>
[DebuggerStepThrough]
public class Failure
{
	/// <summary>
	/// The date and time when the failure instance was created.
	/// </summary>
	public DateTime Created { get; } = DateTime.Now;

	/// <summary>
	/// The failure message.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// The actual value that caused the assertion to fail.
	/// </summary>
	public object? Actual { get; }

	/// <summary>
	/// The expected value that was compared during the assertion and caused the failure.
	/// </summary>
	public object? Expected { get; }

	/// <summary>
	/// The name of the assertion that failed.
	/// </summary>
	public string? Assertion { get; }

	/// <summary>
	/// The name of the method that called the assertion, or null if unavailable.
	/// </summary>
	public string? Method { get; }

	/// <summary>
	/// The name of the file in which the exception occurred, if available.
	/// </summary>
	public string? File { get; }

	/// <summary>
	/// The line number in the source file where the exception occurred.
	/// </summary>
	public int? Line { get; }

	/// <summary>
	/// The specific line of source code of the assertion failure.
	/// </summary>
	public string? Code { get; }

	public Type? CustomExceptionType { get; init; }

	public List<Failure>? Failures { get; }

	public Failure(string message, object? actual = null, object? expected = null, List<Failure>? failures = null, bool subFailure = false)
	{
		Message = failures == null ? message : $"{message}\n\n\t{string.Join("\n\t", failures.Select(f => f.Message).ToList().Truncate(30))}\n";

		Actual = actual;
		Expected = expected;

		Failures = failures?.ToList();

		if (subFailure)
			return;

		var (codeFrame, assertionFrame) = FindFrames();

		Assertion = assertionFrame?.GetMethod()?.Name;

		Method = codeFrame?.GetMethod()?.Name;
		File = codeFrame?.GetFileName();
		Line = codeFrame?.GetFileLineNumber();
		Code = File?.GetLine(Line!.Value);

		Message = Message.AppendCodeLine(codeFrame);
	}

	public override string ToString() => Message;

	private static (StackFrame? code, StackFrame? assertion) FindFrames()
	{
		if (!Configuration.Active.AppendCodeLine)
			return (null, null);

		var frames = new StackTrace(true).GetFrames();
		var codeFrame = frames.FindFrame();

		return (codeFrame, frames[Array.IndexOf(frames, codeFrame) - 1]);
	}
}