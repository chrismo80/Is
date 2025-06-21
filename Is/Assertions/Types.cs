using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Types
{
	/// <summary>
	/// Asserts that the actual object is of type <typeparamref name="T" />.
	/// </summary>
	/// <returns>The cast object to the type <typeparamref name="T" />.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static T? Is<T>(this object actual) => Check
		.That(actual, obj => obj is T)
		.Yields(obj => (T)obj)
		.Unless(actual, "is no", typeof(T));

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this object actual) => Check
		.That(actual is not T)
		.Unless(actual, "is a", typeof(T));
}