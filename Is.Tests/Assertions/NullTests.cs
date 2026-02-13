using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class NullTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

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
}
