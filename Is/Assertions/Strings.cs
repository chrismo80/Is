using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Is;

[DebuggerStepThrough]
public static class Strings
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// contains the specified <paramref name="expected"/> substring.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining(this string actual, string expected) => Check
		.That(actual).And(expected)
		.Return(() => actual.Contains(expected))
		.OrFailWith("is not containing");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// starts with the specified <paramref name="expected"/> string.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsStartingWith(this string actual, string expected) => Check
		.That(actual).And(expected)
		.Return(() => actual.StartsWith(expected))
		.OrFailWith("is not starting with");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// ends with the specified <paramref name="expected"/> string.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEndingWith(this string actual, string expected) => Check
		.That(actual).And(expected)
		.Return(() => actual.EndsWith(expected))
		.OrFailWith("is not ending with");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// matches the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static GroupCollection? IsMatching(this string actual, string pattern) => Check
		.That(actual).And(pattern)
		.Return(() => Regex.Match(actual, pattern) is { Success: true } match ? match.Groups : null)
		.OrFailWith("is not matching");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// does not match the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotMatching(this string actual, string pattern)=> Check
		.That(actual).And(pattern)
		.Return(() => !Regex.Match(actual, pattern).Success)
		.OrFailWith("is matching");
}