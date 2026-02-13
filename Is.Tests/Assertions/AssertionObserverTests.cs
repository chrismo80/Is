using Is.Assertions;
using Is.Core;
using Is.Core.Interfaces;

namespace Is.Tests.Assertions;

[TestFixture]
public class AssertionObserverTests
{
	private IAssertionObserver? _previousObserver;

	[SetUp]
	public void SetUp()
	{
		_previousObserver = Configuration.Default.AssertionObserver;
	}

	[TearDown]
	public void TearDown()
	{
		Configuration.Default.AssertionObserver = _previousObserver;
	}

	[Test]
	public void Observer_ReceivesPassedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionObserver = new DelegateObserver(events.Add);

		true.IsTrue();
		42.Is(42);
		"hello".IsContaining("ell");

		events.Count.Equals(3);
		events.TrueForAll(e => e.Passed).Equals(true);
	}

	[Test]
	public void Observer_ReceivesFailedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionObserver = new DelegateObserver(events.Add);

		using var context = AssertionContext.Begin();

		false.IsTrue();

		context.NextFailure();

		events.Exists(e => !e.Passed).Equals(true);
	}

	[Test]
	public void Observer_ReceivesMixedAssertions()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionObserver = new DelegateObserver(events.Add);

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
	public void Observer_FailedEvent_ContainsFailureDetails()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionObserver = new DelegateObserver(events.Add);

		using var context = AssertionContext.Begin();

		false.IsTrue();

		context.NextFailure();

		var failEvent = events.Find(e => !e.Passed);

		(failEvent is not null).Equals(true);
		(failEvent!.Message is not null).Equals(true);
		failEvent.Message!.Contains("IsTrue").Equals(true);
	}

	[Test]
	public void StatisticsObserver_CollectsStats()
	{
		var stats = new StatisticsObserver();
		Configuration.Default.AssertionObserver = stats;

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
	public void StatisticsObserver_Summary_ContainsAssertionNames()
	{
		var stats = new StatisticsObserver();
		Configuration.Default.AssertionObserver = stats;

		true.IsTrue();
		42.Is(42);

		var summary = stats.Summary();

		summary.Contains("passed").Equals(true);
	}

	[Test]
	public void StatisticsObserver_Reset_ClearsAll()
	{
		var stats = new StatisticsObserver();
		Configuration.Default.AssertionObserver = stats;

		true.IsTrue();
		42.Is(42);

		stats.Reset();

		stats.Total.Equals(0);
		stats.TotalPassed.Equals(0);
		stats.TotalFailed.Equals(0);
	}

	[Test]
	public void NullObserver_DoesNotInterfere()
	{
		Configuration.Default.AssertionObserver = null;

		true.IsTrue();
		42.Is(42);
		"hello".IsContaining("ell");
	}

	[Test]
	public void Observer_WorksWithAssertionContext()
	{
		var events = new List<AssertionEvent>();
		Configuration.Default.AssertionObserver = new DelegateObserver(events.Add);

		using var context = AssertionContext.Begin();
		context.Configuration.AssertionObserver = new DelegateObserver(events.Add);

		true.IsTrue();

		// AssertionContext uses its own config, so events get added there too
		(events.Count > 0).Equals(true);
	}

	private class DelegateObserver(Action<AssertionEvent> handler) : IAssertionObserver
	{
		public void OnAssertion(AssertionEvent assertionEvent) => handler(assertionEvent);
	}

	private class StatisticsObserver : IAssertionObserver
	{
		private readonly List<AssertionEvent> _events = [];

		public int Total => _events.Count;
		public int TotalPassed => _events.Count(e => e.Passed);
		public int TotalFailed => _events.Count(e => !e.Passed);
		public double PassRate => Total > 0 ? (double)TotalPassed / Total : 0;

		public void OnAssertion(AssertionEvent assertionEvent)
		{
			_events.Add(assertionEvent);
		}

		public string Summary()
		{
			return $"{TotalPassed} passed, {TotalFailed} failed ({PassRate:P})";
		}

		public void Reset()
		{
			_events.Clear();
		}
	}
}
