using System.Collections.Concurrent;
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
		Configuration.Active.TestAdapter.ReportSuccess();

		AssertionContext.Current?.AddSuccess();

		return result;
	}

	internal static T? Failed<T>(string message, object? actual = null, object? expected = null)
	{
		var failure = new Failure(message, actual, expected);

		if (Configuration.Active.ThrowOnFailure && !AssertionContext.IsActive)
			Configuration.Active.TestAdapter.ReportFailure(failure);

		AssertionContext.Current?.AddFailure(failure);

		Configuration.Active.Logger(failure.Message);

		return default;
	}

	internal static T? Failed<T>(object? actual, string equality, object? expected) =>
		Failed<T>(actual.Actually(equality, expected), actual, expected);

	internal static T? Failed<T>(object? actual, string equality) =>
		Failed<T>(actual.Actually(equality), actual);

	internal static T? Failed<T>(string message, List<string> text, int max = 100) =>
		Failed<T>($"{message}\n\n\t{string.Join("\n\t", text.Truncate(max))}\n");
}

[DebuggerStepThrough]
file static class StackFrameExtensions
{
	private static readonly Assembly Mine = Assembly.GetExecutingAssembly();

	internal static StackFrame? FindFrame(this StackFrame[] frames) =>
		frames.FirstOrDefault(f => f.IsExtensionCall() && f.GetFileName() != null);

	private static bool IsExtensionCall(this StackFrame frame) =>
		(frame.GetMethod()?.IsForeignAssembly() ?? false) && (frame.GetMethod()?.HasNotAttribute() ?? false);

	private static bool IsForeignAssembly(this MethodBase method) =>
		method.DeclaringType?.Assembly != Mine && !Attribute.IsDefined(method, typeof(IsAssertionAttribute));

	private static bool HasNotAttribute(this MethodBase method) =>
		!Attribute.IsDefined(method, typeof(IsAssertionAttribute)) && !Attribute.IsDefined(method.DeclaringType, typeof(IsAssertionsAttribute));
}