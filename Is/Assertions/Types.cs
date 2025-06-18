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
			return cast;

		new NotException(actual, "is no", typeof(T)).Throw();

		return default;
	}

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNot<T>(this object actual)
	{
		if (actual is not T)
			return true;

		return new NotException(actual, "is a", typeof(T)).Throw();
	}
}