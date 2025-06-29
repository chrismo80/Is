using System.Diagnostics;
using System.Reflection;
using Is.Tools;

namespace Is.Core;

[DebuggerStepThrough]
internal static class Assertion
{
	internal static bool Passed() => Passed(true);

	internal static T Passed<T>(T result) => result;

	internal static T? Failed<T>(Failure failure)
	{
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
}