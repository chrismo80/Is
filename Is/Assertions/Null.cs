using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Null
{
	/// <summary>
	/// Asserts that an object is <c>null</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNull(this object? actual) =>
		actual.IsExactly(null);

	/// <summary>
	/// Asserts that the object is not <c>null</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotNull(this object? actual)
	{
		if(actual is not null)
			return true;

		throw new NotException(actual, "is", null);
	}
}