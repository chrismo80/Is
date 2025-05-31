namespace Is.Assertions;

public static class Null
{
	/// <summary>
	/// Asserts that an object is <c>null</c>.
	/// </summary>
	public static bool IsNull(this object? actual) =>
		actual.IsExactly(null);

	/// <summary>
	/// Asserts that the object is not <c>null</c>.
	/// </summary>
	public static bool IsNotNull(this object? actual) =>
		(actual is not null).ThrowIf(actual, "is null");
}