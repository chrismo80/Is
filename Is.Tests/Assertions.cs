using System.Runtime.CompilerServices;
using Is.Core;
using Is.Core.TestAdapters;
using Is.Assertions;
using Is.Tools;

namespace Is.Tests;

[TestFixture]
public class Assertions
{
	private static int DivideByZero(int value) => value / 0;

	private static async Task<int> WaitAndDivide(int value)
	{
		await Task.Delay(10);

		if (value < 5)
			return DivideByZero(value);

		throw new InvalidOperationException("nope, timeout");
	}

	[Test]
	public void IsThrowing_Sync()
	{
		Action action = () => DivideByZero(1);

		action.IsThrowing<DivideByZeroException>();
		action.IsThrowing<DivideByZeroException>("by zero");
		action.IsNotThrowing<ArgumentException>();
	}

	[Test]
	public void IsNotThrowing_Sync()
	{
		var action = () => Console.WriteLine("Hello World");

		action.IsNotThrowing<DivideByZeroException>();
		action.IsNotThrowing<ArgumentException>();
	}

	[Test]
	public void IsThrowing_NotThrown_Sync()
	{
		var action = () => Task.Delay(10).Wait();
		Action outerAction = () => action.IsThrowing<DivideByZeroException>();

		outerAction.IsThrowing<NotException>("not thrown");
	}

	[Test]
	public void IsThrowing_Async()
	{
		var action1 = () => WaitAndDivide(4);

		action1.IsThrowing<DivideByZeroException>();
		action1.IsThrowing<DivideByZeroException>("by zero");

		var action2 = () => WaitAndDivide(7);

		action2.IsThrowing<InvalidOperationException>();
		action2.IsThrowing<InvalidOperationException>("timeout");

		action1.IsNotThrowing<InvalidOperationException>();
		action2.IsNotThrowing<DivideByZeroException>();
	}

	[Test]
	public void IsThrowing_NotThrown_Async()
	{
		var action = () => Task.Delay(100);
		Action failingAction = () => action.IsThrowing<DivideByZeroException>();
		Action succeedingAction = () => action.IsNotThrowing<DivideByZeroException>();


		action.IsCompletingWithin(TimeSpan.FromMilliseconds(200));
		action.IsNotThrowing<Exception>();
		failingAction.IsThrowing<NotException>("not thrown");
		succeedingAction.IsCompletingWithin(TimeSpan.FromMilliseconds(200));
	}

	[Test]
	public void Is_Type()
	{
		new List<int>().Is<IReadOnlyList<int>>();
		"hello".Is<string>();
		5.5.Is<double>();
		5.Is<int>();

		Action action = () => "hello".Is<int>();
		action.IsThrowing<NotException>();

		"hello".IsNot<int>();

		var ex = new ArgumentException("hello", new InvalidCastException("world"));

		ex.InnerException.Is<InvalidCastException>();
	}

	[Test]
	public void Is_NotType()
	{
		new List<int>().IsNot<IReadOnlyList<double>>();

		Action action = () => new List<int>().Is<IReadOnlyList<double>>();
		action.IsThrowing<NotException>();
	}

	[Test]
	[TestCase(null, null)]
	[TestCase(false, false)]
	[TestCase(true, true)]
	[TestCase(1, 1)]
	[TestCase(2.2, 2.2)]
	[TestCase(3f, 3f)]
	[TestCase("4", "4")]
	public void IsExactly_ActualEqualsExpected_NotThrowing(object? actual, object? expected)
	{
		actual.IsExactly(expected);
		actual.Is(expected);
	}

	[Test]
	[TestCase(true, null)]
	[TestCase(false, null)]
	[TestCase(true, false)]
	[TestCase(5, null)]
	[TestCase(6, 6d)]
	[TestCase(7, true)]
	[TestCase(false, 7d)]
	[TestCase(8d, 8f)]
	[TestCase(99, "99")]
	[TestCase("ABC", false)]
	[TestCase("ABC", null)]
	[TestCase("ABC", "ABD")]
	public void IsExactly_ActualNotEqualsExpected_Throwing(object? actual, object? expected)
	{
		actual.IsNot(expected);
		actual.IsNotNull();

		((Action)(() => actual.IsExactly(expected))).IsThrowing<NotException>();
		((Action)(() => actual.Is(expected))).IsThrowing<NotException>();
	}

	[Test]
	public void Is_Array()
	{
		int?[] array = [1, 2, null, 4];

		array.Is(1, 2, null, 4);
		array.Is((int?[]) [1, 2, null, 4]);
		array.Is(new List<int?> { 1, 2, null, 4 });
		array.Where(i => i % 2 == 0).Is(2, 4);

		Action action = () => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });
		action.IsThrowing<NotException>("is not");
	}

	[Test]
	[AssertionContext]
	public void Is_NotNullOrEmpty()
	{
		string? test = null;
		test.IsNotNullOrEmpty();

		test = "";
		test.IsNotNullOrEmpty();

		test = " ";
		test.IsNotNullOrEmpty();

		AssertionContext.Current?.TakeFailures(2);
	}

	[Test]
	public void Is_List()
	{
		((int?[])[1, 2, null, 4]).Is(1, 2, null, 4);

		List<int?> list = [1, 2, null, 4];

		list.Is(1, 2, null, 4);
		list.Is((int?[]) [1, 2, null, 4]);
		list.Is(new List<int?> { 1, 2, null, 4 });
		list.Where(i => i % 2 == 0).Is(2, 4);

		Action action = () => new List<int?> { 1, 2, null, 4 }.Is(new List<int?> { 1, 2, 3, 4 });
		action.IsThrowing<NotException>();

	}

	[Test]
	public void IsAllocating()
	{
		byte[] buffer = [];

		Action action = () => buffer = new byte[1024 * 1024 * 10]; // 10 MB total

		action.IsAllocatingAtMost(10_300);

		Action pass = () => action.IsAllocatingAtMost(11_000);
		Action fail = () => action.IsAllocatingAtMost(10_000);

		pass.IsNotThrowing<NotException>();
		fail.IsThrowing<NotException>();
	}

	[Test]
	public void Dictionary()
	{
		var dict1 = new Dictionary<int, double>() { [0] = 0.0, [1] = 1.0, [2] = 2.0 };
		var dict2 = new Dictionary<int, double>() { [1] = 11.0, [2] = 2.0, [3] = 3.0 };
		var dict3 = new Dictionary<int, double>() { [1] = 11.0, [2] = 2.0, [3] = 3.0 };
		var dict4 = new Dictionary<int, double>() { [1] = 11.0, [2] = 22.0, [3] = 3.0 };

		dict2.IsEquivalentTo(dict3);
		dict2.IsMatching(dict3);
		dict2.Is(dict3);

		dict3.IsEquivalentTo(dict4, key => key == 2);

		var actions = new List<Action>()
		{
			() => dict1.IsEquivalentTo(dict2),
			() => dict3.IsEquivalentTo(dict4),
		};

		foreach(var action in actions)
			action.IsThrowing<NotException>();
	}

	[Test]
	[AssertionContext]
	public void Matching()
	{
		var expectedSnapshot = new
			{
				Name = "Test",
				Value = 123,
				Info = new
				{
					WrongItem = 5.0,
					Details = new
					{
						Names = new[] { "Lorem", "Ipsum" },
					}
				},
				Tags = new[] { "tag1", "tag2" },
				Max = 4.5
			};

		var actualObject = new
		{
			Name = "Test",
			Value = 123,
			Info = new
			{
				WrongItem = 5.0,
				Details = new
				{
					Names = new[] { "Lorem", "Ipsum" },
				}
			},
			Tags = new[] { "tag1", "tag2" },
			Max = 4.5
		};

		var failingObject = new
		{
			Name = "Test",
			Value = 456,
			Info = new
			{
				WrongItem = 5,
				NewItem = "ABC",
				Details = new
				{
					Names = new[] { "Ipsum", "Lorem" },
				}
			},
			Tags = new[] { "tag1", "tag2", "tag3" },
			Min = 1.2
		};

		actualObject.IsMatching(expectedSnapshot);
		actualObject.IsMatchingSnapshot(expectedSnapshot);

		Configuration.Active.ColorizeMessages = false;

		failingObject.IsMatching(expectedSnapshot);
		failingObject.IsMatchingSnapshot(expectedSnapshot);

		AssertionContext.Current?.NextFailure().SaveJson("Match.json");
		AssertionContext.Current?.NextFailure().SaveJson("Snapshot.json");
	}

	private class MyObject
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public override string ToString() => $"{Id}:{Name}";
	}

	[Test]
	[AssertionContext]
	public void IsDeeplyEquivalentTo()
	{
		var list1 = new List<MyObject> { new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" } };
		var list2 = new List<MyObject> { new() { Id = 2, Name = "B" }, new() { Id = 1, Name = "A" } };
		var list3 = new List<MyObject> { new() { Id = 1, Name = "A" }, new() { Id = 3, Name = "C" } };

		list1.IsEquivalentTo(list2);		// ❌
		list1.IsDeeplyEquivalentTo(list2);	// ✅
		list1.IsDeeplyEquivalentTo(list3);	// ❌

		AssertionContext.Current?.TakeFailures(2).Count.Is(2);
	}

	[Test]
	public void Is_DifferentArrayDepths()
	{
		new object[] { (int[]) [1, 2], 3 }.Is(new object[] { (int[]) [1, 2], 3 });

		new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

		Action action = () => new List<object> { 1, 2 }.IsExactly(new List<object> { 1, new List<object> { 2 } });
		action.IsThrowing<NotException>();
	}

	[Test]
	[AssertionContext]
	public void Is_IEnumerable_TooShort()
	{
		new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);

		AssertionContext.Current?.NextFailure().SaveJson("Is_IEnumerable_TooShort.json");
	}

	[Test]
	public void Is_IEnumerable_TooLong()
	{
		Action action = () => new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);
		action.IsThrowing<NotException>("are not");
	}

	[Test]
	public void IsContaining()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.IsContaining(2, 4);

		"hello world".IsNotNullOrEmpty();
		"hello world".IsContaining("hello");
		"hello world".IsStartingWith("hello");
		"hello world".IsEndingWith("world");

		new List<int> { 1, 2, 3, 4 }.IsContaining(1);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 3);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2, 3);
	}

	[Test]
	public void IsUnique()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.IsUnique();

		Action action = () => new List<int> { 1, 2, 3, 2, 5, 6 }.IsUnique();
		action.IsThrowing<NotException>("is containing a duplicate");
	}

	[Test]
	public void IsMatching()
	{
		"hello world".IsMatching("hello");
		var groups = "hello world".IsMatching("(.*) (.*)");

		groups[1].Value.Is("hello");
		groups[2].Value.Is("world");

		"hello".IsNotMatching("world");

		Action action1 = () => "hello".IsMatching("world");
		action1.IsThrowing<NotException>("is not matching");

		Action action2 = () => "hello".IsNotMatching("hello");
		action2.IsThrowing<NotException>("is matching");
	}

	[Test]
	public void Is_IEnumerable()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);
	}

	[Test]
	public void IsEquivalentTo_IsSatisfying()
	{
		List<int> list1 = [1, 2, 3, 4];
		List<int> list2 = [3, 2, 4, 1];
		List<int> list3 = [3, 5, 4, 1];
		List<int> list4 = [1, 2, 3];
		List<int> list5 = [1, 2, 3, 4, 5];

		list1.IsSatisfying(l => l.All(x => x.IsPositive()));

		list1.IsEquivalentTo(list2);
		list2.IsEquivalentTo(list1);

		list1.IsSatisfying(l => l.All(x => x < 5));

		var actions = new List<Action>
		{
			() => list2.IsEquivalentTo(list3),
			() => list1.IsEquivalentTo(list4),
			() => list1.IsEquivalentTo(list5),
			() => list1.IsSatisfying(l => l.All(x => x.IsNegative()))
		};

		foreach(var action in actions)
			action.IsThrowing<NotException>();
	}

	[Test]
	public void Is_ValueNotEqualsList_Throwing()
	{
		Action action = () => 5.Is(new List<int> { 1, 2 });
		action.IsThrowing<NotException>();
	}

	[Test]
	public void Comparisons()
	{
		var (min, max) = (-11, 33);

		min.IsNegative();
		max.IsPositive();

		(-11).IsInRange(min, max);
		(33).IsInRange(min, max);
		(0).IsInRange(min, max);

		(-11).IsNotBetween(min, max);
		(33).IsNotBetween(min, max);
		(0).IsBetween(min, max);

		(-12).IsOutOfRange(min, max);
		(34).IsOutOfRange(min, max);
	}

	[Test]
	[TestCase(3, 4)]
	[TestCase(5.7, 9.5)]
	[TestCase(-4, -2)]
	[TestCase(-1, 1)]
	[TestCase(0, 5)]
	public void IsGreaterThan_IsSmallerThan<T>(T actual, T expected) where T : IComparable<T>
	{
		actual.IsSmallerThan(expected);
		expected.IsGreaterThan(actual);

		Action action1 = () => expected.IsSmallerThan(actual);
		action1.IsThrowing<NotException>("is not smaller than");

		Action action2 = () => actual.IsGreaterThan(expected);
		action2.IsThrowing<NotException>("is not greater than");
	}


	[Test]
	[TestCase(1, 6)]
	[TestCase(2, 7)]
	[TestCase(3, 8)]
	public void IsAtLeast_IsAtMost(int min, int max)
	{
		min.IsAtLeast(1);
		min.IsAtMost(3);

		max.IsAtLeast(6);
		max.IsAtMost(8);

		3.IsInRange(min, max);
		4.IsInRange(min, max);
		5.IsInRange(min, max);
		6.IsInRange(min, max);

		4.IsBetween(min, max);
		5.IsBetween(min, max);
	}

	[Test]
	[TestCase(1, 2, 3)]
	[TestCase(-1.0, 0.0, 1.0)]
	[TestCase("A", "B", "C")]
	public void IsBetween_NotThrowing<T>(T min, T mid, T max) where T : IComparable<T>
	{
		mid.IsBetween(min, max);

		min.IsNotBetween(mid, max);
		max.IsNotBetween(min, mid);

		min.IsOutOfRange(mid, max);
		max.IsOutOfRange(min, mid);

		Action action = () => mid.IsNotBetween(min, max);
		action.IsThrowing<NotException>("is between");
	}

	[Test]
	public void IsThrowing_WithInnerException()
	{
		Action action = () => throw new InvalidOperationException("invalid", new ArgumentException("arg"));

		action.IsThrowing<InvalidOperationException>().InnerException.Is<ArgumentException>().Message.Is("arg");
	}

	[Test]
	public void DateTime()
	{
		var from = new DateTime(2025, 05, 24, 11, 11, 10);
		var to = new DateTime(2025, 05, 24, 11, 11, 11);

		from.IsSmallerThan(to);
		to.IsGreaterThan(from);
	}

	[Test]
	public void IsIn()
	{
		new List<int> { 1 }.IsIn(1, 2, 3, 4);
		new List<int> { 1, 2 }.IsIn(1, 2, 3, 4);
		new List<int> { 1, 2, 3 }.IsIn(1, 2, 3, 4);
	}

	[Test]
	public void IsTrue_IsFalse()
	{
		true.IsTrue();
		false.IsFalse();

		((Action)(() => true.IsFalse())).IsThrowing<NotException>("is not");
		((Action)(() => false.IsTrue())).IsThrowing<NotException>("is not");
	}

	[Test]
	public void DateTimes_TimeSpans()
	{
		var from = new DateTime(2025, 05, 24, 11, 11, 10);
		var to = new DateTime(2025, 05, 25, 11, 10, 10);

		from.IsApproximately(to, TimeSpan.FromDays(1));

		var duration = to - from;

		duration.IsApproximately(TimeSpan.FromDays(1), TimeSpan.FromMinutes(1));
	}

	[Test]
	public void IsEmpty()
	{
		new List<int>().IsEmpty();

		((Action)(() => new List<int> {1, 2}.IsEmpty())).IsThrowing<NotException>("is not");
	}


	[Test]
	public void Guards()
	{
		int value;
		Action action;

		value = 5;

		action = () => Check.That(value < 0).Unless<ArgumentException>("oh oh");
		action.IsThrowing<ArgumentException>("oh oh");

		action = () => Check.That(value < 0).Unless<InvalidOperationException>("nope");
		action.IsThrowing<InvalidOperationException>("nope");

		action = () => Check.Arg(value < 0, "oh no");
		action.IsThrowing<ArgumentException>("oh no");

		action = () => Check.Op(value < 0, "oh no");
		action.IsThrowing<InvalidOperationException>("oh no");
	}

	[Test]
	public void IsNull_IsDefault_IsSame()
	{
		List<int>? list = null;
		list.IsNull();
		list.IsDefault();

		Action action = () => list.IsNotNull();
		action.IsThrowing<NotException>();

		list = new List<int>() {1, 2};
		list.IsNotNull();

		action = () => list.IsNull();
		action.IsThrowing<NotException>("is not");

		action = () => list.IsDefault();
		action.IsThrowing<NotException>("is not");

		var list2 = new List<int>() { 1, 2 };
		var list3 = list;

		list.Is(list2);
		list.IsSameAs(list3);

		action = () => list.IsSameAs(list2);
		action.IsThrowing<NotException>("is not the same");
	}

	[Test]
	[TestCase(1.000001, 1.0)]
	[TestCase(2.999999, 3)]
	[TestCase(1000000.1, 1000000.0)]
	[TestCase(1_000_000.0, 1_000_001.0)]
	[TestCase(783.0123, 783.0124)]
	[TestCase(1.0 / 3.0, 0.333333)]
	[TestCase(0.1 + 0.2, 0.3)]
	public void IsCloseTo_Actual_Expected(double actual, double expected)
	{
		actual.IsApproximately(expected, 1e-4);
		actual.IsApproximately(expected);
		actual.Is(expected);
		actual.IsPositive();

		((float)actual).IsApproximately((float)expected);

		Action action = () => actual.IsExactly(expected);
		action.IsThrowing<NotException>().Message.Contains("is not").IsTrue();

		action = () => actual.IsNegative();
		action.IsThrowing<NotException>("is not smaller");

		Configuration.Active.FloatingPointComparisonPrecision = 1e-4;
		actual.IsApproximately(expected);
	}

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

		AssertionContext.Current?.TakeFailures(12);

		false.IsTrue(); // ❌
		4.Is(5); // ❌

		// Verify expected count and dequeue failures
		AssertionContext.Current?.TakeFailures(2)
			.All(failure => failure.Message.IsContaining("is not")); // ✅
	}

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
	}

	[Test]
	public void Check_That_Unless()
	{
		const bool value = true;

		Check.That(value).Unless(value, "is false");
	}

	[Test]
	[AssertionContext]
	public void CustomAssertion()
	{
		(5 - 9).IsCustomAssertion();

		AssertionContext.Current?.NextFailure().SaveJson("custom.json");
	}
}

[IsAssertions]
public static class CustomAssertions
{
	public static bool IsCustomAssertion(this int value, [CallerArgumentExpression("value")] string? expr = null) =>
		Check.That(value > 0).Unless(value, $"in '{expr}' is not positive");
}