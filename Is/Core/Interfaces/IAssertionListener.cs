namespace Is.Core.Interfaces;

/// <summary>
/// Interface for observing all assertion outcomes — both passed and failed.
/// Unlike <see cref="IFailureObserver"/> which only sees failures,
/// an <see cref="IAssertionListener"/> is notified of every assertion evaluation.
/// Can be set via Configuration.AssertionListener.
/// </summary>
public interface IAssertionListener
{
	/// <summary>
	/// Called after every assertion evaluation, regardless of outcome.
	/// </summary>
	void OnAssertion(AssertionEvent assertionEvent);
}
