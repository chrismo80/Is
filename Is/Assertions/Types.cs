using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Is.Assertions;

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
			return cast;

		throw new NotException(actual, "is no", typeof(T));
	}

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this object actual)
	{
		if (actual is not T)
			return true;

		throw new NotException(actual, "is", typeof(T));
	}
}