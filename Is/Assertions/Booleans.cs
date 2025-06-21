using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Booleans
{
	/// <summary>
	/// Asserts that a boolean value is <c>true</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsTrue(this bool actual) =>
		actual.IsExactly(true);

	/// <summary>
	/// Asserts that a boolean value is <c>false</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsFalse(this bool actual) =>
		actual.IsExactly(false);
}