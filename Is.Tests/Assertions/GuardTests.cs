using Is.Assertions;
using Is.Core;
using Is.TestAdapters;
using Is.Tools;

namespace Is.Tests.Assertions;

[TestFixture]
public class GuardTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

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
	public void Check_That_Unless()
	{
		const bool value = true;

		Check.That(value).Unless(value, "is false");
	}
}
