using Is.Assertions;
using Is.Core;
using Is.AssertionObservers;
using Is.TestAdapters;
using System.Reflection;
using System.Text;

namespace Is.Tests.Assertions;

[TestFixture]
public class ConfigurationTests
{
	private string? _localConfigPath;

	[SetUp]
	public void SetUp()
	{
		_localConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Is.Tests.is.configuration.json");
		File.Delete(_localConfigPath);
		Configuration.ResetCache();
	}

	[TearDown]
	public void TearDown()
	{
		if (_localConfigPath is not null)
			File.Delete(_localConfigPath);

		Configuration.ResetCache();
		Configuration.Default.TestAdapter = new DefaultAdapter();
		Configuration.Active.TestAdapter = new DefaultAdapter();
		Configuration.Default.AssertionObserver = null;
		Configuration.Default.AppendCodeLine = true;
		Configuration.Default.ColorizeMessages = true;
		Configuration.Default.FloatingPointComparisonPrecision = 1e-6;
		Configuration.Default.MaxRecursionDepth = 20;
		Configuration.Default.ParsingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
	}

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
	public void Configuration_AssemblyLocal_Overrides_Global()
	{
		Configuration.Default.FloatingPointComparisonPrecision = 0.5;
		File.WriteAllText(_localConfigPath!, """
		{
		  "FloatingPointComparisonPrecision": 0.01,
		  "AppendCodeLine": false
		}
		""", Encoding.UTF8);
		Configuration.ResetCache();

		var assemblyConfiguration = Configuration.ResolveFor(Assembly.GetExecutingAssembly());

		assemblyConfiguration.FloatingPointComparisonPrecision.Is(0.01);
		assemblyConfiguration.AppendCodeLine.IsFalse();
		assemblyConfiguration.ColorizeMessages.IsTrue();
	}

	[Test]
	public void Configuration_AssemblyLocal_Is_Cached()
	{
		File.WriteAllText(_localConfigPath!, """
		{
		  "FloatingPointComparisonPrecision": 0.01
		}
		""", Encoding.UTF8);
		Configuration.ResetCache();

		var assembly = Assembly.GetExecutingAssembly();
		var first = Configuration.ResolveFor(assembly);

		File.WriteAllText(_localConfigPath!, """
		{
		  "FloatingPointComparisonPrecision": 0.02
		}
		""", Encoding.UTF8);

		var second = Configuration.ResolveFor(assembly);

		ReferenceEquals(first, second).IsTrue();
		second.FloatingPointComparisonPrecision.Is(0.01);
	}

	[Test]
	public void Configuration_AssertionContext_Overrides_AssemblyLocal()
	{
		File.WriteAllText(_localConfigPath!, """
		{
		  "FloatingPointComparisonPrecision": 0.01
		}
		""", Encoding.UTF8);
		Configuration.ResetCache();

		using var context = AssertionContext.Begin();
		AssertionContext.Current?.Configuration.FloatingPointComparisonPrecision.Is(0.01);

		Configuration.Active.FloatingPointComparisonPrecision = 0.02;

		AssertionContext.Current?.Configuration.FloatingPointComparisonPrecision.Is(0.02);
		Configuration.ResolveFor(Assembly.GetExecutingAssembly()).FloatingPointComparisonPrecision.Is(0.01);
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
		using (var context = AssertionContext.Begin())
		{
			Configuration.Active.TestAdapter = new NUnitTestAdapter();
			Configuration.Active.TestAdapter.Is<NUnitTestAdapter>();

			try { 5.0.IsExactly(6.0); }
			catch (Exception ex) { ex.Is<AssertionException>(); }

			context.NextFailure();
		}

		using (var context = AssertionContext.Begin())
		{
			Configuration.Active.TestAdapter = new DefaultAdapter();
			Configuration.Active.TestAdapter.Is<DefaultAdapter>();

			try { 5.0.IsExactly(6.0); }
			catch (Exception ex) { ex.Is<NotException>(); }

			context.NextFailure();
		}

		using (var context = AssertionContext.Begin())
		{
			Configuration.Active.TestAdapter = new UnitTestAdapter();
			Configuration.Active.TestAdapter.Is<UnitTestAdapter>();

			try { 5.0.IsExactly(6.0); }
			catch (Exception ex) { ex.Is<AssertionException>(); }

			try { 5.0.IsExactly(6.0); }
			catch (Exception ex) { ex.Is<AssertionException>(); }

			context.TakeFailures(2);
		}

		using (var context = AssertionContext.Begin())
		{
			Configuration.Active.TestAdapter =
				new CustomExceptionAdapter<ArgumentException>(failure => new ArgumentException(failure.Message));

			try { 5.0.IsExactly(6.0); }
			catch (Exception ex) { ex.Is<ArgumentException>(); }

			context.NextFailure();
		}
	}
}