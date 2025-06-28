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
		AssertionContext.Current?.AddSuccess();

		Configuration.Active.TestAdapter.ReportSuccess();

		return result;
	}

	internal static T? Failed<T>(string message, object? actual = null, object? expected = null)
	{
		var failure = new Failure(message, actual, expected);

		if (AssertionContext.IsActive)
			AssertionContext.Current?.AddFailure(failure);
		else
			Configuration.Active.TestAdapter.ReportFailure(failure);

		return default;
	}

	internal static T? Failed<T>(object? actual, string equality, object? expected) =>
		Failed<T>(actual.Actually(equality, expected), actual, expected);

	internal static T? Failed<T>(object? actual, string equality) =>
		Failed<T>(actual.Actually(equality), actual);

	internal static T? Failed<T>(string message, List<string> text, int max = 100) =>
		Failed<T>($"{message}\n\n\t{string.Join("\n\t", text.Truncate(max))}\n");
}