namespace Is.Assertions;

public static class Booleans
{
	/// <summary>
	/// Asserts that a boolean value is <c>true</c>.
	/// </summary>
	public static bool IsTrue(this bool actual) =>
		actual.IsExactly(true);

	/// <summary>
	/// Asserts that a boolean value is <c>false</c>.
	/// </summary>
	public static bool IsFalse(this bool actual) =>
		actual.IsExactly(false);
}