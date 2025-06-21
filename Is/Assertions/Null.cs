using System.Runtime.CompilerServices;
using System.Diagnostics;

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
	public static bool IsNotNull(this object? actual) =>
		actual.IsNot(null);
}