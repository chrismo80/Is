using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class StringTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[Test]
	[AssertionContext]
	public void Is_NotBlank()
	{
		string? test = null;
		test.IsNotBlank();

		test = "";
		test.IsNotBlank();

		test = " ";
		test.IsNotBlank();

		AssertionContext.Current?.TakeFailures(3);
	}

	[Test]
	[AssertionContext]
	public void IsEquivalentTo_Strings()
	{
		"hello".IsEquivalentTo("Hello");

		"hello".IsEquivalentTo("hallo");
		AssertionContext.Current?.NextFailure();

		"AbC".IsEquivalentTo("aBc");
	}

	[Test]
	[AssertionContext]
	public void IsEmail()
	{
		"pre.post@provider.com".IsEmail();

		"pre.post_provider.com".IsEmail();
		AssertionContext.Current?.NextFailure();
	}

	[Test]
	public void IsMatching()
	{
		"hello world".IsMatching("hello");
		var groups = "hello world".IsMatching("(.*) (.*)");

		groups[1].Value.Is("hello");
		groups[2].Value.Is("world");

		"hello".IsNotMatching("world");

		Action action1 = () => "hello".IsMatching("world");
		action1.IsThrowing<NotException>("is not matching");

		Action action2 = () => "hello".IsNotMatching("hello");
		action2.IsThrowing<NotException>("is matching");
	}
}
