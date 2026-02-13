using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class EqualityTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

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
	public void Is_ValueNotEqualsList_Throwing()
	{
		Action action = () => 5.Is(new List<int> { 1, 2 });
		action.IsThrowing<NotException>();
	}
}
