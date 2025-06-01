using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Strings
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// contains the specified <paramref name="expected"/> substring.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining(this string actual, string expected)
	{
		if(actual.Contains(expected))
			return true;

		throw new NotException(actual, "is not containing", expected);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// matches the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static GroupCollection IsMatching(this string actual, string pattern)
	{
		if (Regex.Match(actual, pattern) is { Success: true } match)
			return match.Groups;

		throw new NotException(actual, "does not match", pattern);
	}

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// does not match the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotMatching(this string actual, string pattern)
	{
		if(!Regex.Match(actual, pattern).Success)
			return true;

		throw new NotException(actual, "is matching", pattern);
	}
}