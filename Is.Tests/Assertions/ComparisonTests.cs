using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class ComparisonTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

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
		expected.IsGreaterThan(actual);
		actual.IsSmallerThan(expected);

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
}