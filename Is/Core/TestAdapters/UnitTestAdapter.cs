using Is.Core.Interfaces;

namespace Is.Core.TestAdapters;

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


	public void ReportFailure(Failure failure)
	{
		if(failure.CustomExceptionType is { } customType)
			ThrowException(failure.Message, customType);

		ThrowException(failure.Message, ExceptionType);
	}

	public void ReportFailures(string message, List<Failure> failures)
	{
		var messages = string.Join("\n\n", failures.Select(f => f.Message));

		ThrowException($"{message}\n{messages}", ExceptionType);
	}

	private static void ThrowException(string message, Type type)
	{
		if (type.GetConstructor([typeof(string)])?.Invoke([message]) is Exception ex)
			throw ex;

		throw new NotException(message);
	}

	private static Type? FindType() => typeNames
		.Select(typeName => Type.GetType(typeName, false))
		.FirstOrDefault(type => type is not null);
}