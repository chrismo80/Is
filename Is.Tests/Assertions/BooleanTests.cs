using Is.Assertions;

namespace Is.Tests.Assertions;

[TestFixture]
public class BooleanTests
{
	[Test]
	public void IsTrue_IsFalse()
	{
		true.IsTrue();
		false.IsFalse();

		((Action)(() => true.IsFalse())).IsThrowing<NotException>("is not");
		((Action)(() => false.IsTrue())).IsThrowing<NotException>("is not");
	}
}
