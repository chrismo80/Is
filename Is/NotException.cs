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
public class NotException(string message, object? actual = null, object? expected = null) : Exception(message.AppendCodeLine())
{
	public object? Actual => actual;
	public object? Expected => expected;
}

[DebuggerStepThrough]
file static class CallStackExtensions
{
	private static readonly Assembly Mine = Assembly.GetExecutingAssembly();

	private static readonly ConcurrentDictionary<string, string[]> SourceCache = new();

	internal static string AppendCodeLine(this string text) =>
		Configuration.AppendCodeLine ? "\n" + text + "\n" + new StackTrace(true).FindFrame()?.CodeLine() + "\n" : text;

	private static StackFrame? FindFrame(this StackTrace trace) =>
		trace.GetFrames().FirstOrDefault(f => f.IsExtensionCall() && f.GetFileName() != null);

	private static bool IsExtensionCall(this StackFrame frame) =>
		(frame.GetMethod()?.IsForeignAssembly() ?? false) && (frame.GetMethod()?.HasNotAttribute() ?? false);

	private static bool IsForeignAssembly(this MethodBase method) =>
		method.DeclaringType?.Assembly != Mine && !Attribute.IsDefined(method, typeof(IsExtensionAttribute));

	private static bool HasNotAttribute(this MethodBase method) =>
		!Attribute.IsDefined(method, typeof(IsExtensionAttribute)) && !Attribute.IsDefined(method.DeclaringType, typeof(IsExtensionsAttribute));

	private static string CodeLine(this StackFrame frame) => "in " +
		frame.GetMethod()?.DeclaringType.Color(1) + frame.GetFileName()?.GetLine(frame.GetFileLineNumber());

	private static string GetLine(this string fileName, int lineNumber) => " in line " + lineNumber.Color(1) + ": " +
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim().Color(93);
}