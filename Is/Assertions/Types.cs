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
	public static T Is<T>(this object actual)
	{
		if (actual is T cast)
			return Assertion.Passed(cast);

		return Assertion.Failed<T>(actual, "is no", typeof(T));
	}

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this object actual)
	{
		if (actual is not T)
			return Assertion.Passed();

		return Assertion.Failed<bool>(actual, "is a", typeof(T));
	}
}