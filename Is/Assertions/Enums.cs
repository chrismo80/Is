using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Enums
{
	/// <summary>
	/// Asserts that the enum value is one of the expected values.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsAnyOf<T>(this T enumValue, params T[] expectedValues) where T : struct, Enum => Check
		.That(expectedValues.Contains(enumValue))
		.Unless(enumValue, "is not in", expectedValues);
}
