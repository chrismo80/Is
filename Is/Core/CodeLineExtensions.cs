using System.Collections.Concurrent;
using System.Diagnostics;
using Is.Tools;

namespace Is.Core;

[DebuggerStepThrough]
internal static class CodeLineExtensions
{
	private static readonly ConcurrentDictionary<string, string[]> SourceCache = new();

	internal static string AppendCodeLine(this string text, StackFrame? frame) =>
		Configuration.Active.AppendCodeLine ? "\n" + text + "\n" + frame?.CodeLine() + "\n" : text;

	private static string CodeLine(this StackFrame frame) =>
		"in " + frame.GetMethod()?.DeclaringType.Color(1) + frame.GetFileName()?.CreateLine(frame.GetFileLineNumber());

	private static string CreateLine(this string fileName, int lineNumber) =>
		" in line " + lineNumber.Color(1) + ": " + fileName.GetLine(lineNumber).Color(93);

	internal static string GetLine(this string fileName, int lineNumber) =>
		SourceCache.GetOrAdd(fileName, File.ReadAllLines)[lineNumber - 1].Trim();
}
