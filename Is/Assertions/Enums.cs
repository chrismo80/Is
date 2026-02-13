using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Enums
{
	/// <summary>
	/// Asserts that the enum value is a defined value of the enum type.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsAnyOf<T>(this T enumValue) where T : struct, Enum => Check
		.That(Enum.IsDefined(typeof(T), enumValue))
		.Unless(enumValue, "is not a defined value of", typeof(T).Name);
}
