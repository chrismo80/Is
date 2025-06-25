using System.Diagnostics;
using System.Reflection;
using System.Collections.Concurrent;
using Is.Core;
using Is.Tools;

namespace Is;

/// <summary>
/// This exception is thrown when an assertion fails and <c>ThrowOnFailure</c> is enabled.
/// When used inside an <see cref="AssertionContext"/>, instances of <see cref="NotException"/>
/// are collected instead of being thrown immediately.
/// </summary>
[DebuggerStepThrough]
public class NotException(string message, StackFrame? frame, object? actual = null, object? expected = null)
	: Exception(message.AppendCodeLine(frame))
{
	/// <summary>
	/// The actual value that caused the assertion to fail.
	/// </summary>
	public object? Actual => actual;

	/// <summary>
	/// The expected value that was compared during the assertion and caused the failure.
	/// </summary>
	public object? Expected => expected;

	/// <summary>
	/// The name of the method where the stack frame originates, or null if unavailable.
	/// </summary>
	public string? Method => frame?.GetMethod()?.Name;

	/// <summary>
	/// The name of the file in which the exception occurred, if available.
	/// </summary>
	public string? File => frame?.GetFileName();

	/// <summary>
	/// The line number in the source file where the exception occurred.
	/// </summary>
	public int? Line => frame?.GetFileLineNumber();

	/// <summary>
	/// The specific line of source code of the assertion failure.
	/// </summary>
	public string? Code => File?.GetLine(Line!.Value);
}

[DebuggerStepThrough]
file static class CodeLineExtensions
{
	private static readonly ConcurrentDictionary<string, string[]> SourceCache = new();

	internal static string AppendCodeLine(this string text, StackFrame? frame) =>
		Configuration.AppendCodeLine ? "\n" + text + "\n" + frame?.CodeLine() + "\n" : text;

	private static string CodeLine(this StackFrame frame) => "in " +
		frame.GetMethod()?.DeclaringType.Color(1) + frame.GetFileName()?.CreateLine(frame.GetFileLineNumber());

	private static string CreateLine(this string fileName, int lineNumber) =>
		" in line " + lineNumber.Color(1) + ": " + fileName.GetLine(lineNumber).Color(93);

	internal static string GetLine(this string fileName, int lineNumber) =>
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim();
}