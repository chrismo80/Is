namespace Is.Tests;

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
	}

	[Test]
	public void IsThrowing_NotThrown_Async()
	{
		var action = () => Task.Delay(10);
		Action outerAction = () => action.IsThrowing<DivideByZeroException>();

		outerAction.IsThrowing<NotException>("not thrown");
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
	public void Is_List()
	{
		List<int?> list = [1, 2, null, 4];

		list.Is(1, 2, null, 4);
		list.Is((int?[]) [1, 2, null, 4]);
		list.Is(new List<int?> { 1, 2, null, 4 });
		list.Where(i => i % 2 == 0).Is(2, 4);

		Action action = () => new List<int?> { 1, 2, null, 4 }.Is(new List<int?> { 1, 2, 3, 4 });
		action.IsThrowing<NotException>();

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
	public void Is_IEnumerable_TooShort()
	{
		Action action = () => new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);
		action.IsThrowing<NotException>("are not");
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

		"hello world".IsContaining("hello");

		new List<int> { 1, 2, 3, 4 }.IsContaining(1);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 3);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2, 3);
	}

	[Test]
	public void IsMatching()
	{
		"hello world".IsMatching("hello");
		var groups = "hello world".IsMatching("(.*) (.*)");

		groups[1].Value.Is("hello");
		groups[2].Value.Is("world");

		"hello".IsNotMatching("world");

		Action action = () => "hello".IsMatching("world");
		action.IsThrowing<NotException>("IsMatching");
	}

	[Test]
	public void Is_IEnumerable()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);
	}


	[Test]
	public void IsEquivalentTo()
	{
		List<int> list1 = [1, 2, 3, 4];
		List<int> list2 = [3, 2, 4, 1];
		List<int> list3 = [3, 5, 4, 1];
		List<int> list4 = [1, 2, 3];
		List<int> list5 = [1, 2, 3, 4, 5];

		list1.IsEquivalentTo(list2);
		list2.IsEquivalentTo(list1);

		var actions = new List<Action>
		{
			() => list2.IsEquivalentTo(list3),
			() => list1.IsEquivalentTo(list4),
			() => list1.IsEquivalentTo(list5),
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
	[TestCase(3, 4)]
	[TestCase(5.7, 9.5)]
	[TestCase(-4, -2)]
	[TestCase(-1, 1)]
	[TestCase(0, 5)]
	public void IsGreaterThan_IsSmallerThan<T>(T actual, T expected) where T : IComparable<T>
	{
		actual.IsSmallerThan(expected);
		expected.IsGreaterThan(actual);

		Action action = () => actual.IsGreaterThan(expected);
		action.IsThrowing<NotException>().Message.Contains("is not greater than").IsTrue();
	}

	[Test]
	[TestCase(1, 2, 3)]
	[TestCase(-1.0, 0.0, 1.0)]
	[TestCase("A", "B", "C")]
	public void IsBetween_NotThrowing<T>(T min, T actual, T max) where T : IComparable<T>
	{
		actual.IsBetween(min, max);

		min.IsNotBetween(actual, max);
		max.IsNotBetween(actual, min);
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
	public void IsEmpty()
	{
		new List<int>().IsEmpty();

		((Action)(() => new List<int> {1, 2}.IsEmpty())).IsThrowing<NotException>("is not");
	}

	[Test]
	public void IsNull()
	{
		List<int>? list = null;
		list.IsNull();
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

		((float)actual).IsApproximately((float)expected);

		Action action = () => actual.IsExactly(expected);
		action.IsThrowing<NotException>().Message.Contains("is not").IsTrue();
	}
}