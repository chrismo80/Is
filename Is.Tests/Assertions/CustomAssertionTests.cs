using Is.Assertions;
using Is.Core;

namespace Is.Tests.Assertions;

[TestFixture]
public class CustomAssertionTests
{
	[Test]
	[AssertionContext]
	public void CustomAssertion()
	{
		(5 - 9).IsCustomAssertion();

		AssertionContext.Current?.NextFailure();
	}
}
