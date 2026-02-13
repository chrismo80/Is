using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class TypeTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

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
}
