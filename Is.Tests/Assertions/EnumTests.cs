using Is.Assertions;

namespace Is.Tests.Assertions;

[TestFixture]
public class EnumTests
{
	[Test]
	public void IsAnyOf_Enum()
	{
		TestEnum.A.IsAnyOf<TestEnum>();
		TestEnum.B.IsAnyOf<TestEnum>();
		TestEnum.C.IsAnyOf<TestEnum>();
	}

	[Test]
	public void IsAnyOf_Enum_Fails()
	{
		var undefinedValue = (TestEnum)999;

		Action action = () => undefinedValue.IsAnyOf<TestEnum>();

		action.IsThrowing<NotException>("is not a defined value of");
	}

	private enum TestEnum
	{
		A,
		B,
		C
	}
}
