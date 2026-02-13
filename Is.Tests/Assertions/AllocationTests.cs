using Is.Assertions;
using Is.Core;
using Is.FailureObservers;

namespace Is.Tests.Assertions;

[TestFixture]
public class AllocationTests
{
	[Test]
	public void IsAllocating()
	{
		byte[] buffer = [];

		Action action = () => buffer = new byte[1024 * 1024 * 10]; // 10 MB total

		action.IsAllocatingAtMost(10_300);

		Action pass = () => action.IsAllocatingAtMost(11_000);
		Action fail = () => action.IsAllocatingAtMost(10_000);

		pass.IsNotThrowing<NotException>();
		fail.IsThrowing<NotException>();
	}
}
