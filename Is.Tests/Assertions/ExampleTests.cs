using System.Runtime.CompilerServices;
using Is.Assertions;
using Is.Core;

namespace Is.Tests.Assertions;

[TestFixture]
public class ExampleTests
{
	[Test]
	[AssertionContext]
	public void Examples()
	{
		((1.0 / 3.0) == 0.33333).IsTrue(); // ❌
		((1.0 / 3.0) == 0.33333).IsFalse(); // ✅

		Enumerable.Range(1, 3).IsUnique(); // ✅
		Enumerable.Range(1, 3).IsEmpty(); // ❌
		Enumerable.Range(1, 3).IsIn(0, 1, 2, 3, 4); // ✅
		Enumerable.Range(1, 3).IsEquivalentTo(Enumerable.Range(1, 3).Reverse()); // ✅

		(1.0 / 3.0).IsApproximately(0.33333); // ❌
		(1.0 / 3.0).IsApproximately(0.33333, 0.01); // ✅

		5.IsBetween(2, 5); // ❌
		5.IsInRange(2, 5); // ✅
		5.IsGreaterThan(5); // ❌
		5.IsAtLeast(5); // ✅

		TimeSpan.Parse("1:23").IsApproximately(TimeSpan.Parse("1:24"), TimeSpan.FromMinutes(1)); // ✅
		TimeSpan.Parse("1:23").IsApproximately(TimeSpan.Parse("1:25"), TimeSpan.FromMinutes(1)); // ❌

		static int DivideByZero(int value) => value / 0;
		Action action1 = () => _ = DivideByZero(1);
		action1.IsThrowing<DivideByZeroException>(); // ✅

		Action action = () => 5.IsGreaterThan(6);
		action.IsNotThrowing<Is.NotException>(); // ❌

		byte[] buffer = [];
		Action action3 = () => buffer = new byte[1024 * 1024 * 10]; // 10 MB
		action3.IsAllocatingAtMost(10_300); // ✅
		action3.IsAllocatingAtMost(10_200); // ❌

		(0.1 + 0.2).IsExactly(0.3); // ❌
		(0.1 + 0.2).Is(0.3); // ✅ (automatically checks Approximately)
		2.999999f.Is(3f); // ✅
		783.0123.Is(783.0124); // ✅

		Enumerable.Range(1, 4).Is(1, 2, 3, 4); // ✅
		Enumerable.Range(1, 4).Where(x => x % 2 == 0).Is(2, 4); // ✅
		Enumerable.Range(1, 4).Where(x => x % 3 == 0).Is(3); // ✅

		List<int>? list = null;
		list.IsNull(); // ✅
		list.IsDefault(); // ✅
		list.IsNotNull(); // ❌

		var groups = "hello world".IsMatching("(.*) (.*)"); // ✅
		groups[1].Value.Is("world"); // ❌
		groups[2].Value.Is("world"); // ✅

		"hello world".IsContaining("hell"); // ✅
		"hello world".IsStartingWith("hell"); // ✅

		"hello".Is<string>(); // ✅
		"hello".Is<int>(); // ❌

		AssertionContext.Current?.TakeFailures(11);

		false.IsTrue(); // ❌
		4.Is(5); // ❌

		// Verify expected count and dequeue failures
		AssertionContext.Current?.TakeFailures(2)
			.All(failure => failure.Message.IsContaining("is")); // ✅
	}
}

[IsAssertions]
public static class CustomAssertions
{
	public static bool IsCustomAssertion(this int value, [CallerArgumentExpression("value")] string? expr = null) =>
		Check.That(value > 0).Unless(value, $"in '{expr}' is not positive");
}
