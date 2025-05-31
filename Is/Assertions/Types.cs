namespace Is.Assertions;

public static class Types
{
	/// <summary>
	/// Asserts that the actual object is of type <typeparamref name="T" />.
	/// </summary>
	/// <returns>The cast object to the type <typeparamref name="T" />.</returns>
	public static T Is<T>(this object actual)
	{
		if (actual is T cast)
			return cast;

		throw new NotException(actual, "is no", typeof(T));
	}

	/// <summary>
	/// Asserts that the actual object is not of type <typeparamref name="T"/>.
	/// </summary>
	public static bool IsNot<T>(this object actual) =>
		(actual is not T).ThrowIf(actual, "is", typeof(T));
}