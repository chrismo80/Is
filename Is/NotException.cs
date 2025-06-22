using System.Diagnostics;
using System.Reflection;
using System.Collections.Concurrent;
using Is.Core;

namespace Is;

/// <summary>
/// This exception is thrown when an assertion fails and <c>ThrowOnFailure</c> is enabled.
/// When used inside an <see cref="AssertionContext"/>, instances of <see cref="NotException"/>
/// are collected instead of being thrown immediately.
/// </summary>
[DebuggerStepThrough]
public class NotException(string message) : Exception(message.AddCodeLine());

[DebuggerStepThrough]
file static class CallStackExtensions
{
	private static readonly Assembly Mine = Assembly.GetExecutingAssembly();

	private static readonly ConcurrentDictionary<string, string[]> SourceCache = new();

	internal static string AddCodeLine(this string text) =>
		Configuration.AppendCodeLine ? "\n" + text + "\n" + new StackTrace(true).FindFrame()?.CodeLine() + "\n" : text;

	private static StackFrame? FindFrame(this StackTrace trace) =>
		trace.EnumerateFrames().FirstOrDefault(f => f?.IsForeignAssembly() == true && f.GetFileName() != null);

	private static IEnumerable<StackFrame?> EnumerateFrames(this StackTrace trace) =>
		Enumerable.Range(0, trace.FrameCount).Select(trace.GetFrame);

	private static bool IsForeignAssembly(this StackFrame frame) =>
		frame.GetMethod()?.DeclaringType?.Assembly != Mine;

	private static string CodeLine(this StackFrame frame) => "in " +
		frame.GetMethod()?.DeclaringType.Color(1) + frame.GetFileName()?.GetLine(frame.GetFileLineNumber());

	private static string GetLine(this string fileName, int lineNumber) => " in line " + lineNumber.Color(1) + ": " +
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim().Color(93);
}