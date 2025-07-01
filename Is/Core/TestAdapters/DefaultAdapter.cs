using Is.Core.Interfaces;

namespace Is.Core.TestAdapters;

public class DefaultAdapter : ITestAdapter
{
	public void ReportFailure(Failure failure) => throw CreateException(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(CreateException));

	private static Exception CreateException(Failure failure)
	{
		if(failure.CustomExceptionType is { } type && Activator.CreateInstance(type, failure.Message) is { } ex)
			return (Exception)ex;

		return new NotException(failure.Message);
	}
}