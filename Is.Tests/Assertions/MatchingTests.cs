using Is.Assertions;
using Is.Core;

namespace Is.Tests.Assertions;

[TestFixture]
public class MatchingTests
{
	private sealed record MatchingRoot(string Name, MatchingChild Child);
	private sealed record MatchingChild(int Value, string[] Tags);
	private readonly record struct MatchingPoint(int X, int Y);
	private sealed record MatchingStructRoot(string Name, MatchingPoint Point);

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

	[Test]
	public void IsMatching_RecordGraph_ReportsDifferencesInsteadOfThrowingReflectionExceptions()
	{
		var expected = new MatchingRoot("Root", new MatchingChild(1, ["A", "B"]));
		var matching = expected with { };
		var actual = new MatchingRoot("Root", new MatchingChild(2, ["A", "B"]));

		matching.IsMatching(expected);

		var exception = ((Action)(() => actual.IsMatching(expected))).IsThrowing<NotException>();

		exception.Message.Contains("TargetInvocationException").IsFalse();
		exception.Message.Contains("Invalid handle").IsFalse();
	}

	[Test]
	public void IsMatching_ReadonlyRecordStructGraph_ReportsDifferencesInsteadOfThrowingReflectionExceptions()
	{
		var expected = new MatchingStructRoot("Root", new MatchingPoint(1, 2));
		var matching = expected with { };
		var actual = new MatchingStructRoot("Root", new MatchingPoint(1, 3));

		matching.IsMatching(expected);

		var exception = ((Action)(() => actual.IsMatching(expected))).IsThrowing<NotException>();

		exception.Message.Contains("TargetInvocationException").IsFalse();
		exception.Message.Contains("Invalid handle").IsFalse();
	}
}