using Is.Core;
using Is.Tools;
using Is.Core.Interfaces;
using System.Diagnostics;

namespace Is.TestAdapters;

/// <summary>
/// <see cref="ITestAdapter"/> that is throwing test framework specific exception.
/// Detects the loaded framework (MS Test, xUnit, NUnit) on first failure.
/// If none of those is detected <see cref="NotException"/>(s) are thrown.
/// </summary>
[DebuggerStepThrough]
public class UnitTestAdapter : ITestAdapter
{
	private static readonly string[] typeNames =
	[
		"Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException, Microsoft.VisualStudio.TestTools.UnitTesting",
		"Xunit.Sdk.XunitException, xunit.assert",
		"NUnit.Framework.AssertionException, nunit.framework",
	];

	private static Type? _detectedType;

	private static Type ExceptionType => _detectedType ??= FindType() ?? typeof(NotException);


	public void ReportFailure(Failure failure) =>
		ThrowException(failure.Message, failure.CustomExceptionType ?? ExceptionType);

	public void ReportFailures(string message, List<Failure> failures) =>
		ThrowException(Combine(message, failures.Select(f => f.Message)), ExceptionType);

	private static void ThrowException(string message, Type type) =>
		throw type.ToInstance<Exception>(message) ?? new NotException(message);

	private static string Combine(string message, IEnumerable<string> messages) =>
		$"{message}\n{string.Join("\n\n", messages)}";

	private static Type? FindType() => typeNames.Select(typeName => typeName.ToType())
		.FirstOrDefault(type => type is not null);
}