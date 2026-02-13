using System.Diagnostics;
using System.Reflection;

namespace Is.Core;

/// <summary>
/// Represents the outcome of an assertion evaluation (passed or failed).
/// Contains all information about the assertion including location, values, and result.
/// </summary>
public sealed class AssertionEvent
{
	/// <summary>Whether the assertion passed.</summary>
	public bool Passed { get; }

	/// <summary>The failure message (null if passed).</summary>
	public string? Message { get; }

	/// <summary>The actual value that was asserted.</summary>
	public object? Actual { get; }

	/// <summary>The expected value, if applicable.</summary>
	public object? Expected { get; }

	/// <summary>The name of the assertion method (e.g. "Is", "IsGreaterThan").</summary>
	public string? Assertion { get; }

	/// <summary>The name of the method that called the assertion.</summary>
	public string? Method { get; }

	/// <summary>The source file where the assertion occurred.</summary>
	public string? File { get; }

	/// <summary>The line number in the source file.</summary>
	public int? Line { get; }

	/// <summary>The source code line content.</summary>
	public string? Code { get; }

	/// <summary>The timestamp when the assertion was evaluated.</summary>
	public DateTime Timestamp { get; } = DateTime.Now;

	/// <summary>Custom exception type for failed assertions.</summary>
	public Type? CustomExceptionType { get; init; }

	/// <summary>Nested failures for collection assertions.</summary>
	public List<AssertionEvent>? InnerEvents { get; }

	/// <summary>Creates a passed assertion event.</summary>
	internal static AssertionEvent CreatePassed(string? assertion, string? file, int? line, string? code)
	{
		return new AssertionEvent(true, null, null, null, assertion, null, file, line, code);
	}

	/// <summary>Creates a failed assertion event.</summary>
	internal static AssertionEvent CreateFailed(string message, object? actual, object? expected,
		string? assertion, string? method, string? file, int? line, string? code)
	{
		return new AssertionEvent(false, message, actual, expected, assertion, method, file, line, code);
	}

	internal AssertionEvent(bool isPassed, string? message, object? actual, object? expected,
		string? assertion, string? method, string? file, int? line, string? code,
		Type? customExceptionType = null,
		List<AssertionEvent>? innerEvents = null)
	{
		Passed = isPassed;
		Message = message;
		Actual = actual;
		Expected = expected;
		Assertion = assertion;
		Method = method;
		File = file;
		Line = line;
		Code = code;
		CustomExceptionType = customExceptionType;
		InnerEvents = innerEvents;
	}

	/// <summary>Returns the message or a default representation.</summary>
	public override string ToString() => Message ?? (Passed ? "Assertion passed" : "Assertion failed");
}
