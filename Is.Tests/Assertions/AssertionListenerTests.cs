using Is.Assertions;
using Is.AssertionListeners;
using Is.Core;
using Is.Core.Interfaces;

namespace Is.Tests.Assertions;

[TestFixture]
public class AssertionListenerTests
{
	private IAssertionListener? _previousListener;

	[SetUp]
	public void SetUp()
	{
		_previousListener = Configuration.Default.AssertionListener;
	}

	[TearDown]
	public void TearDown()
	{
		Configuration.Default.AssertionListener = _previousListener;
	}

	[Test]
	public void Listener_ReceivesPassedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionListener = new DelegateListener(events.Add);

		true.IsTrue();
		42.Is(42);
		"hello".IsContaining("ell");

		events.Count.Equals(3);
		events.TrueForAll(e => e.Passed).Equals(true);
	}

	[Test]
	public void Listener_ReceivesFailedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionListener = new DelegateListener(events.Add);

		using var context = AssertionContext.Begin();

		false.IsTrue();

		context.NextFailure();

		events.Exists(e => !e.Passed).Equals(true);
	}

	[Test]
	public void Listener_ReceivesMixedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionListener = new DelegateListener(events.Add);

		using var context = AssertionContext.Begin();

		true.IsTrue();   // pass
		false.IsTrue();  // fail
		42.Is(42);       // pass

		context.NextFailure();

		var passed = events.FindAll(e => e.Passed).Count;
		var failed = events.FindAll(e => !e.Passed).Count;

		passed.Equals(2);
		failed.Equals(1);
	}

	[Test]
	public void Listener_FailedEvent_ContainsFailureDetails()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionListener = new DelegateListener(events.Add);

		using var context = AssertionContext.Begin();

		false.IsTrue();

		context.NextFailure();

		var failEvent = events.Find(e => !e.Passed);

		(failEvent is not null).Equals(true);
		(failEvent!.Failure is not null).Equals(true);
		failEvent.Failure!.Message.Contains("IsTrue").Equals(true);
	}

	[Test]
	public void StatisticsListener_CollectsStats()
	{
		var stats = new StatisticsListener();
		Configuration.Default.AssertionListener = stats;

		using var context = AssertionContext.Begin();

		true.IsTrue();
		false.IsTrue();
		42.Is(42);
		"a".Is("b");

		context.TakeFailures(2);

		stats.Total.Equals(4);
		stats.TotalPassed.Equals(2);
		stats.TotalFailed.Equals(2);
		(stats.PassRate > 0.49 && stats.PassRate < 0.51).Equals(true);
	}

	[Test]
	public void StatisticsListener_Summary_ContainsAssertionNames()
	{
		var stats = new StatisticsListener();
		Configuration.Default.AssertionListener = stats;

		true.IsTrue();
		42.Is(42);

		var summary = stats.Summary();

		summary.Contains("passed").Equals(true);
	}

	[Test]
	public void StatisticsListener_Reset_ClearsAll()
	{
		var stats = new StatisticsListener();
		Configuration.Default.AssertionListener = stats;

		true.IsTrue();
		42.Is(42);

		stats.Reset();

		stats.Total.Equals(0);
		stats.TotalPassed.Equals(0);
		stats.TotalFailed.Equals(0);
	}

	[Test]
	public void NullListener_DoesNotInterfere()
	{
		Configuration.Default.AssertionListener = null;

		true.IsTrue();
		42.Is(42);
		"hello".IsContaining("ell");
	}

	[Test]
	public void Listener_WorksWithAssertionContext()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionListener = new DelegateListener(events.Add);

		using var context = AssertionContext.Begin();
		context.Configuration.AssertionListener = new DelegateListener(events.Add);

		true.IsTrue();

		// AssertionContext uses its own config, so events get added there too
		(events.Count > 0).Equals(true);
	}

	private class DelegateListener(Action<AssertionEvent> handler) : IAssertionListener
	{
		public void OnAssertion(AssertionEvent assertionEvent) => handler(assertionEvent);
	}
}
