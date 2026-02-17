using Is.Assertions;
using Is.Core;

namespace Is.Tests.Assertions;

[TestFixture]
public class DateTimeTests
{
	[Test]
	[AssertionContext]
	public void IsOlderThan()
	{
		var birthDate = new DateTime(2007, 07, 06, 11, 11, 11);
		var checkDate = new DateTime(2025, 07, 05, 11, 11, 11);

		birthDate.IsOlderThan(18, checkDate);
		AssertionContext.Current?.NextFailure();

		birthDate.IsOlderThan(18);

		birthDate.IsSameDay(new DateTime(2007, 07, 06, 23, 11, 11));
		birthDate.IsSameDay(checkDate);
		AssertionContext.Current?.NextFailure();

		new DateTime(2000, 2, 29).IsOlderThan(3, new DateTime(2003, 3, 1));
		new DateTime(2001, 3, 1).IsOlderThan(2, new DateTime(2004, 2, 29));
	}

	[Test]
	public void DateTime_Comparisons()
	{
		var from = new DateTime(2025, 05, 24, 11, 11, 10);
		var to = new DateTime(2025, 05, 24, 11, 11, 11);

		from.IsSmallerThan(to);
		to.IsGreaterThan(from);
	}

	[Test]
	public void DateTimes_TimeSpans()
	{
		var from = new DateTime(2025, 05, 24, 11, 11, 10);
		var to = new DateTime(2025, 05, 25, 11, 10, 10);

		from.IsApproximately(to, TimeSpan.FromDays(1));

		var duration = to - from;

		duration.IsApproximately(TimeSpan.FromDays(1), TimeSpan.FromMinutes(1));
	}
}