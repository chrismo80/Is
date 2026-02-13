using Is.Assertions;
using Is.Core;
using Is.FailureObservers;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class ConfigurationTests
{
	[Test]
	[AssertionContext]
	public void Configuration_Local_Global()
	{
		AssertionContext.Current?.Configuration.FloatingPointComparisonPrecision.Is(0.000001);
		100.1.IsApproximately(100); // ❌
		AssertionContext.Current?.NextFailure();

		Configuration.Active.FloatingPointComparisonPrecision = 0.01;

		AssertionContext.Current?.Configuration.FloatingPointComparisonPrecision.Is(0.01);
		100.1.IsApproximately(100); // ✅
	}

	[Test]
	[AssertionContext]
	public void JsonObserver()
	{
		Configuration.Active.AssertionObserver = new JsonObserver();

		var t = Parallel.For(1, 20, i =>
		{
			(i % 2 == 0).IsTrue();
			i.Is(2);
			3.Is(i);
		});

		AssertionContext.Current?.TakeFailures(46);
	}

	[Test]
	public void TestAdapter()
	{
		Configuration.Active.TestAdapter = new NUnitTestAdapter();
		Configuration.Active.TestAdapter.Is<NUnitTestAdapter>();

		try { 5.0.IsExactly(6.0); }
		catch (Exception ex) { ex.Is<AssertionException>(); }

		Configuration.Active.TestAdapter = new DefaultAdapter();
		Configuration.Active.TestAdapter.Is<DefaultAdapter>();

		try { 5.0.IsExactly(6.0); }
		catch (Exception ex) { ex.Is<NotException>(); }

		Configuration.Active.TestAdapter = new UnitTestAdapter();
		Configuration.Active.TestAdapter.Is<UnitTestAdapter>();

		try { 5.0.IsExactly(6.0); }
		catch (Exception ex) { ex.Is<AssertionException>(); }

		try { 5.0.IsExactly(6.0); }
		catch (Exception ex) { ex.Is<AssertionException>(); }

		Configuration.Active.TestAdapter =
			new CustomExceptionAdapter<ArgumentException>(failure => new ArgumentException(failure.Message));

		try { 5.0.IsExactly(6.0); }
		catch (Exception ex) { ex.Is<ArgumentException>(); }
	}
}
