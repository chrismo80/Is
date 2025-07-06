using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Strings
{
	/// <summary>
	/// Asserts that the <paramref name="actual"/> string is <c>null</c> or
	/// empty or contains only whitespaces.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsBlank(this string actual) => Check
		.That(string.IsNullOrWhiteSpace(actual))
		.Unless(actual, "is not blank");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string is not <c>null</c> and
	/// not empty and does not contain only whitespaces
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotBlank(this string actual) =>
		actual.IsNotNull() && actual.IsNotEmpty() && Check
			.That(!string.IsNullOrWhiteSpace(actual))
			.Unless(actual, "is blank");

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// contains the specified <paramref name="expected"/> substring.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsContaining(this string actual, string expected) => Check
		.That(actual.Contains(expected))
		.Unless(actual, "is not containing", expected);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// starts with the specified <paramref name="expected"/> string.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsStartingWith(this string actual, string expected) => Check
		.That(actual.StartsWith(expected))
		.Unless(actual, "is not starting with", expected);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// ends with the specified <paramref name="expected"/> string.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEndingWith(this string actual, string expected) => Check
		.That(actual.EndsWith(expected))
		.Unless(actual, "is not ending with", expected);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string is equivalent to the
	/// <paramref name="expected"/> string, ignoring case differences.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsEquivalentTo(this string actual, string expected) =>
		actual.ToLower().IsExactly(expected.ToLower());

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// matches the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	/// <returns>The <see cref="GroupCollection"/> of the match if the string matches the pattern.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static GroupCollection? IsMatching(this string actual, string pattern) => Check
		.That(Regex.Match(actual, pattern), match => match.Success)
		.Yields(match => match.Groups)
		.Unless(actual, "is not matching", pattern);

	/// <summary>
	/// Asserts that the <paramref name="actual"/> string
	/// does not match the specified <paramref name="pattern"/> regular expression.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotMatching(this string actual, string pattern)=> Check
		.That(!Regex.Match(actual, pattern).Success)
		.Unless(actual, "is matching", pattern);
}