using System.Diagnostics;
using Is.Tools;

namespace Is.Core;

[DebuggerStepThrough]
internal static class Assertion
{
	internal static bool Passed() => Passed(true);

	internal static T Passed<T>(T result)
	{
		AssertionContext.Current?.AddSuccess();

		return result;
	}

	internal static T? Failed<T>(string message)
	{
		var ex = new NotException(message);

		if (Configuration.ThrowOnFailure && !AssertionContext.IsActive)
			throw ex;

		AssertionContext.Current?.AddFailure(ex);

		Configuration.Logger?.Invoke(ex.Message);

		return default;
	}

	internal static T? Failed<T>(object? actual, string equality, object? expected) =>
		Failed<T>(actual.Actually(equality, expected));

	internal static T? Failed<T>(object? actual, string equality) =>
		Failed<T>(actual.Actually(equality));

	internal static T? Failed<T>(string message, List<string> text, int max = 100) =>
		Failed<T>($"{message}\n\n\t{string.Join("\n\t", text.Truncate(max))}\n");
}