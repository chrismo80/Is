using System.Text.RegularExpressions;

namespace Is.Assertions;

public static class Strings
{
	/// <summary>Asserts that the <paramref name="actual"/> string contains the specified <paramref name="expected"/> substring.</summary>
	public static bool IsContaining(this string actual, string expected) =>
		actual.Contains(expected).ThrowIf(actual, "is not containing", expected);

	/// <summary>Asserts that the <paramref name="actual"/> string matches the specified <paramref name="pattern"/> regular expression.</summary>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	public static GroupCollection IsMatching(this string actual, string pattern)
	{
		if (Regex.Match(actual, pattern) is { Success: true } match)
			return match.Groups;

		throw new NotException(actual, "does not match", pattern);
	}

	/// <summary>Asserts that the <paramref name="actual"/> string does not match the specified <paramref name="pattern"/> regular expression.</summary>
	public static bool IsNotMatching(this string actual, string pattern) =>
		(!Regex.Match(actual, pattern).Success).ThrowIf(actual, "is matching", pattern);
}