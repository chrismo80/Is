using Is.Assertions;
using Is.Core;

namespace Is.Tests.Assertions;

[TestFixture]
public class ContextTests
{
	[Test]
	public void ContextTest_WithUsing()
	{
		try
		{
			using var ctx = AssertionContext.Begin();

			"abc".IsContaining("xyz"); // ❌
			42.Is(0);                  // ❌
		}
		catch (AggregateException ex)
		{
			ex.InnerExceptions.Count.Is(2);
		}

		using var context = AssertionContext.Begin();

		true.IsTrue();  // ✅
		false.IsTrue(); // ❌
		4.Is(5);        // ❌

		context.NextFailure().Message.IsContaining("false.IsTrue()");
		context.NextFailure().Message.IsContaining("4.Is(5)");
	}

	[Test]
	[AssertionContext]
	public void ContextTest_WithAttribute()
	{
		true.IsTrue();  // ✅
		false.IsTrue(); // ❌
		4.Is(5);        // ❌
		5.Is(5);        // ✅
		6.Is(6);        // ✅

		AssertionContext.Current?.NextFailure();
		var failure = AssertionContext.Current?.NextFailure();

		failure.Actual.Is(4);
		failure.Expected.Is(5);
	}
}
