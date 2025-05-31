namespace Is.Assertions;

public static class Equality
{
	/// <summary>
	/// Asserts that the actual object is equal to the expected value.
	/// (no array unwrapping, exact match for floating points)
	/// </summary>
	public static bool IsExactly<T>(this T actual, T expected) =>
		actual.IsExactlyEqualTo(expected).ThrowIf(actual, "is not", expected);

	/// <summary>
	/// Asserts that the actual object matches the expected value(s).
	/// (array unwrapping, approximately for floating points)
	/// </summary>
	public static bool Is(this object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	/// <summary>
	/// Asserts that the actual value is not equal to the expected value.
	/// </summary>
	public static bool IsNot<T>(this T actual, T expected) =>
		(!actual.IsExactlyEqualTo(expected)).ThrowIf(actual, "is", expected);
}