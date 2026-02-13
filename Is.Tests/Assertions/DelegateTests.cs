using Is.Assertions;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class DelegateTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

	private static int DivideByZero(int value) => value / 0;

	private static async Task<int> WaitAndDivide(int value)
	{
		await Task.Delay(10);

		if (value < 5)
			return DivideByZero(value);

		throw new InvalidOperationException("nope, timeout");
	}

	[Test]
	public void IsThrowing_Sync()
	{
		Action action = () => DivideByZero(1);

		action.IsThrowing<DivideByZeroException>();
		action.IsThrowing<DivideByZeroException>("by zero");
		action.IsNotThrowing<ArgumentException>();
	}

	[Test]
	public void IsNotThrowing_Sync()
	{
		var action = () => Console.WriteLine("Hello World");

		action.IsNotThrowing<DivideByZeroException>();
		action.IsNotThrowing<ArgumentException>();
	}

	[Test]
	public void IsThrowing_NotThrown_Sync()
	{
		var action = () => Task.Delay(10).Wait();
		Action outerAction = () => action.IsThrowing<DivideByZeroException>();

		outerAction.IsThrowing<NotException>("not thrown");
	}

	[Test]
	public void IsThrowing_Async()
	{
		var action1 = () => WaitAndDivide(4);

		action1.IsThrowing<DivideByZeroException>();
		action1.IsThrowing<DivideByZeroException>("by zero");

		var action2 = () => WaitAndDivide(7);

		action2.IsThrowing<InvalidOperationException>();
		action2.IsThrowing<InvalidOperationException>("timeout");

		action1.IsNotThrowing<InvalidOperationException>();
		action2.IsNotThrowing<DivideByZeroException>();
	}

	[Test]
	public void IsThrowing_NotThrown_Async()
	{
		var action = () => Task.Delay(100);
		Action failingAction = () => action.IsThrowing<DivideByZeroException>();
		Action succeedingAction = () => action.IsNotThrowing<DivideByZeroException>();


		action.IsCompletingWithin(TimeSpan.FromMilliseconds(200));
		action.IsNotThrowing<Exception>();
		failingAction.IsThrowing<NotException>("not thrown");
		succeedingAction.IsCompletingWithin(TimeSpan.FromMilliseconds(200));
	}

	[Test]
	public void IsThrowing_WithInnerException()
	{
		Action action = () => throw new InvalidOperationException("invalid", new ArgumentException("arg"));

		action.IsThrowing<InvalidOperationException>().InnerException.Is<ArgumentException>().Message.Is("arg");
	}
}