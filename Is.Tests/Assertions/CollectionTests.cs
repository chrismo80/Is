using Is.Assertions;
using Is.Core;
using Is.TestAdapters;

namespace Is.Tests.Assertions;

[TestFixture]
public class CollectionTests
{
	[SetUp]
	public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[TearDown]
	public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

	[Test]
	public void Is_Array()
	{
		int?[] array = [1, 2, null, 4];

		array.Is(1, 2, null, 4);
		array.Is((int?[]) [1, 2, null, 4]);
		array.Is(new List<int?> { 1, 2, null, 4 });
		array.Where(i => i % 2 == 0).Is(2, 4);

		Action action = () => new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });
		action.IsThrowing<NotException>("is not");
	}

	[Test]
	public void Is_List()
	{
		((int?[])[1, 2, null, 4]).Is(1, 2, null, 4);

		List<int?> list = [1, 2, null, 4];

		list.Is(1, 2, null, 4);
		list.Is((int?[]) [1, 2, null, 4]);
		list.Is(new List<int?> { 1, 2, null, 4 });
		list.Where(i => i % 2 == 0).Is(2, 4);

		Action action = () => new List<int?> { 1, 2, null, 4 }.Is(new List<int?> { 1, 2, 3, 4 });
		action.IsThrowing<NotException>();

	}

	[Test]
	public void Is_DifferentArrayDepths()
	{
		new object[] { (int[]) [1, 2], 3 }.Is(new object[] { (int[]) [1, 2], 3 });

		new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

		Action action = () => new List<object> { 1, 2 }.IsExactly(new List<object> { 1, new List<object> { 2 } });
		action.IsThrowing<NotException>();
	}

	[Test]
	[AssertionContext]
	public void Is_IEnumerable_TooShort()
	{
		new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);

		AssertionContext.Current?.NextFailure();
	}

	[Test]
	public void Is_IEnumerable_TooLong()
	{
		Action action = () => new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);
		action.IsThrowing<NotException>("are not");
	}

	[Test]
	public void Is_IEnumerable()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);
	}

	[Test]
	public void IsContaining()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.IsContaining(2, 4);

		"hello world".IsNotBlank();
		"hello world".IsContaining("hello");
		"hello world".IsStartingWith("hello");
		"hello world".IsEndingWith("world");

		new List<int> { 1, 2, 3, 4 }.IsContaining(1);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 3);
		new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2, 3);
	}

	[Test]
	public void IsUnique()
	{
		new List<int> { 1, 2, 3, 4, 5, 6 }.IsUnique();

		Action action = () => new List<int> { 1, 2, 3, 2, 5, 6 }.IsUnique();
		action.IsThrowing<NotException>("is containing a duplicate");
	}

	[Test]
	[AssertionContext]
	public void IsOrdered()
	{
		((List<int>)[-2, -1, 2, 3, 3, 5, 4]).IsOrdered();
		AssertionContext.Current?.NextFailure();

		((List<int>)[-1, 2, 2, 3, 3, 4, 5]).IsOrdered();
	}

	[Test]
	public void IsIn()
	{
		new List<int> { 1 }.IsIn(1, 2, 3, 4);
		new List<int> { 1, 2 }.IsIn(1, 2, 3, 4);
		new List<int> { 1, 2, 3 }.IsIn(1, 2, 3, 4);
	}

	[Test]
	public void IsEmpty()
	{
		new List<int>().IsEmpty();

		((Action)(() => new List<int> {1, 2}.IsEmpty())).IsThrowing<NotException>("is not");
	}

	[Test]
	public void Dictionary()
	{
		var dict1 = new Dictionary<int, double>() { [0] = 0.0, [1] = 1.0, [2] = 2.0 };
		var dict2 = new Dictionary<int, double>() { [1] = 11.0, [2] = 2.0, [3] = 3.0 };
		var dict3 = new Dictionary<int, double>() { [1] = 11.0, [2] = 2.0, [3] = 3.0 };
		var dict4 = new Dictionary<int, double>() { [1] = 11.0, [2] = 22.0, [3] = 3.0 };

		dict2.IsEquivalentTo(dict3);
		dict2.IsMatching(dict3);
		dict2.Is(dict3);

		dict3.IsEquivalentTo(dict4, key => key == 2);

		var actions = new List<Action>()
		{
			() => dict1.IsEquivalentTo(dict2),
			() => dict3.IsEquivalentTo(dict4),
		};

		foreach(var action in actions)
			action.IsThrowing<NotException>();
	}

	private class MyObject
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public override string ToString() => $"{Id}:{Name}";
	}

	[Test]
	[AssertionContext]
	public void IsDeeplyEquivalentTo()
	{
		var list1 = new List<MyObject> { new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" } };
		var list2 = new List<MyObject> { new() { Id = 2, Name = "B" }, new() { Id = 1, Name = "A" } };
		var list3 = new List<MyObject> { new() { Id = 1, Name = "A" }, new() { Id = 3, Name = "C" } };

		list1.IsEquivalentTo(list2);		// ❌
		list1.IsDeeplyEquivalentTo(list2);	// ✅
		list1.IsDeeplyEquivalentTo(list3);	// ❌

		AssertionContext.Current?.TakeFailures(2).Count.Is(2);
	}

	[Test]
	public void IsEquivalentTo_IsSatisfying()
	{
		List<int> list1 = [1, 2, 3, 4];
		List<int> list2 = [3, 2, 4, 1];
		List<int> list3 = [3, 5, 4, 1];
		List<int> list4 = [1, 2, 3];
		List<int> list5 = [1, 2, 3, 4, 5];

		list1.IsSatisfying(l => l.All(x => x.IsPositive()));

		list1.IsEquivalentTo(list2);
		list2.IsEquivalentTo(list1);

		list1.IsSatisfying(l => l.All(x => x < 5));

		var actions = new List<Action>
		{
			() => list2.IsEquivalentTo(list3),
			() => list1.IsEquivalentTo(list4),
			() => list1.IsEquivalentTo(list5),
			() => list1.IsSatisfying(l => l.All(x => x.IsNegative()))
		};

		foreach(var action in actions)
			action.IsThrowing<NotException>();
	}
}