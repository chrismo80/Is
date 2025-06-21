using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public static class Types
{
	/// <summary>
	/// Asserts that the actual object is of type <typeparamref name="T" />.
	/// </summary>
	/// <returns>The cast object to the type <typeparamref name="T" />.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static T? Is<T>(this object actual) => Check
		.That(actual).And(typeof(T))
		.Return(() => actual is T cast ? cast : default)
		.OrFailWith("is no");

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this object actual) => Check
		.That(actual).And(typeof(T))
		.Return(() => actual is not T)
		.OrFailWith("is a");
}