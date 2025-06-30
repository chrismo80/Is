namespace Is.Core.Interfaces;

/// <summary>
/// Represents an interface for handling test result reporting.
/// Serves as a hook for custom test frameworks to throw custom exception types.
/// The provided default adapter throws exceptions for test failures,
/// specifically a <see cref="NotException"/> for single failures
/// and a <see cref="AggregateException"/> for multiple failures.
/// Can be set via Configuration.TestAdapter.
/// </summary>
public interface ITestAdapter
{
	/// <summary>Reports a failed test result to the configured test adapter.</summary>
	void ReportFailure(Failure failure);

	/// <summary>Reports multiple test failures to the configured test adapter.</summary>
	void ReportFailures(string message, List<Failure> failures);
}

public class DefaultAdapter : ITestAdapter
{
	public void ReportFailure(Failure failure) => throw CreateException(failure);

	public void ReportFailures(string message, List<Failure> failures) =>
		throw new AggregateException(message, failures.Select(CreateException));

	private static Exception CreateException(Failure failure)
	{
		if(failure.CustomExceptionType is { } type && Activator.CreateInstance(type, failure.Message) is { } ex)
			return (Exception)ex;

		return new NotException(failure);
	}
}