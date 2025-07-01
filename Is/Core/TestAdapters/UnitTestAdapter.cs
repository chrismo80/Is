using Is.Core.Interfaces;

namespace Is.Core.TestAdapters;

public class UnitTestAdapter : ITestAdapter
{
	private static readonly string[] exceptionTypeNames =
	[
		"Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException, Microsoft.VisualStudio.TestTools.UnitTesting",
		"Xunit.Sdk.XunitException, xunit.assert",
		"NUnit.Framework.AssertionException, nunit.framework",
		"Is.NotException, is",
	];

	private static Type? _exceptionType;

	public static Type ExceptionType => _exceptionType ??= GetFrameworkExceptionType();

	public void ReportFailure(Failure failure) =>
		throw CreateException(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(CreateException));

	private static Exception CreateException(Failure failure)
	{
		if(failure.CustomExceptionType is { } customType)
			return New(customType, failure);

		return New(ExceptionType, failure);
	}

	private static Exception New(Type type, Failure failure) =>
		(Exception)Activator.CreateInstance(type, failure.Message);

	private static Type GetFrameworkExceptionType() =>
		Type.GetType(GetTypeName()) ?? typeof(NotException);

	private static string GetTypeName() =>
		exceptionTypeNames.First(e => Type.GetType(e, false) is not null);
}