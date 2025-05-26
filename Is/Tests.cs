namespace Is;

public class Tests
{
	[Test]
	[TestCase(null, null)]
	[TestCase(false, false)]
	[TestCase(true, true)]
	[TestCase(1, 1)]
	[TestCase(2.2, 2.2)]
	[TestCase(3f, 3f)]
	[TestCase("4", "4")]
	public void Is_Actual_Equals_Expected(object? actual, object? expected)
	{
		actual.Is(expected);
		actual.IsExactly(expected);
	}

	[Test]
	[TestCase(null, true)]
	[TestCase(null, false)]
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
	public void Actual_Not_Equals_Expected(object? actual, object? expected)
	{
		Action act = () => actual.Is(expected);
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void ListValues_Equal_Expected() =>
		VerifyEquality(new List<int> { 1, 2, 3, 4 });

	[Test]
	public void ArrayValues_Equal_Expected() =>
		VerifyEquality(new int[] { 1, 2, 3, 4 });

	[Test]
	public void ValuesWithNull_Equal_Expected() =>
		new int?[] { 1, 2, null, 4 }.Is(1, 2, null, 4);

	[Test]
	public void List_Not_Equal_List()
	{
		Action act = () => new List<int?> { 1, 2, null, 4 }.Is(new List<int?> { 1, 2, 3, 4 });
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void Array_Not_Equal_Params()
	{
		Action act = () => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void IEnumerable_Not_Equal_Params_TooShort()
	{
		Action act = () => new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void IEnumerable_Not_Equal_Params_TooLong()
	{
		Action act = () => new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void IEnumerable_Equal_Params()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);
	}

	[Test]
	public void IsThrowing_Action()
	{
		static int DivideByZero(int value) => value / 0;
		Action action = () => _ = DivideByZero(1);
		action.IsThrowing<DivideByZeroException>();
	}

	[Test]
	public void IsThrowing_AwaitAction()
	{
		static async Task WaitAndThrow()
		{
			await Task.Delay(10);
			throw new InvalidOperationException("nope");
		}

		var action = () => WaitAndThrow();
		action.IsThrowing<InvalidOperationException>().Message.Is("nope");
	}

	[Test]
	public void JaggedArrays_Equals_Expected() =>
		new object[] { new[] { 1, 2 }, 3 }.Is(new object[] { new[] { 1, 2 }, 3 });

	[Test]
	public void DifferentDepth_EqualsThough_Expected() =>
		new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

	[Test]
	public void DifferentDepth_EqualsExactly_Fails()
	{
		Action act = () => new List<object> { 1, 2 }.IsExactly(new List<object> { 1, new List<object> { 2 } });
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void Value_NotEquals_List()
	{
		Action act = () => 5.Is(new List<int> { 1, 2 });
		act.IsThrowing<IsNotException>();
	}

	[Test]
	public void Value_Is_Type() =>
		new List<int>().Is<IReadOnlyList<int>>();

	[Test]
	public void Value_IsNot_Type()
	{
		Action act = () => new List<int>().Is<IReadOnlyList<double>>();
		act.IsThrowing<IsNotException>();
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

		Action act = () => actual.IsGreaterThan(expected);
		act.IsThrowing<IsNotException>().Message.Contains("is not greater than").IsTrue();
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
	public void Booleans()
	{
		true.IsTrue();
		false.IsFalse();
	}

	[Test]
	public void IsEmpty()
	{
		var list = new List<int>();
		list.IsEmpty();
	}

	[Test]
	public void IsNull()
	{
		List<int>? list = null;
		list.IsNull();
	}

	[Test]
	[TestCase(1.000001, 1.0)]
	[TestCase(2.999999f, 3f)]
	[TestCase(1000000.1, 1000000.0)]
	[TestCase(1_000_000.0, 1_000_001.0)]
	[TestCase(783.0123, 783.0124)]
	[TestCase(1.0 / 3.0, 0.333333)]
	[TestCase(0.1 + 0.2, 0.3)]
	public void IsCloseTo_Actual_Expected(object actual, object expected)
	{
		actual.Is(expected);

		Action action = () => actual.IsExactly(expected);
		action.IsThrowing<IsNotException>().Message.Contains("is not").IsTrue();
	}

	private static void VerifyEquality(IEnumerable<int> values)
	{
		values.Is(new int[] { 1, 2, 3, 4 });
		values.Is(new List<int> { 1, 2, 3, 4 });
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}
}