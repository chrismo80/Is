using Is.Assertions;
using Is.Core;
using Is.FailureObservers;

namespace Is.Tests.Assertions;

[TestFixture]
public class MatchingTests
{
	[Test]
	[AssertionContext]
	public void Matching()
	{
		var expectedSnapshot = new
			{
				Name = "Test",
				Value = 123,
				Info = new
				{
					WrongItem = 5.0,
					Details = new
					{
						Names = new[] { "Lorem", "Ipsum" },
					}
				},
				Tags = new[] { "tag1", "tag2" },
				Max = 4.5
			};

		var actualObject = new
		{
			Name = "Test",
			Value = 123,
			Info = new
			{
				WrongItem = 5.0,
				Details = new
				{
					Names = new[] { "Lorem", "Ipsum" },
				}
			},
			Tags = new[] { "tag1", "tag2" },
			Max = 4.5
		};

		var failingObject = new
		{
			Name = "Test",
			Value = 456,
			Info = new
			{
				WrongItem = 5,
				NewItem = "ABC",
				Details = new
				{
					Names = new[] { "Ipsum", "Lorem" },
				}
			},
			Tags = new[] { "tag1", "tag2", "tag3" },
			Min = 1.2
		};

		actualObject.IsMatching(expectedSnapshot);
		actualObject.IsMatchingSnapshot(expectedSnapshot);

		Configuration.Active.ColorizeMessages = false;

		failingObject.IsMatching(expectedSnapshot);
		failingObject.IsMatchingSnapshot(expectedSnapshot);

		AssertionContext.Current?.NextFailure();
		AssertionContext.Current?.NextFailure();
	}
}
