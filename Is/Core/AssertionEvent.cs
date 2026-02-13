namespace Is.Core;

/// <summary>
/// Represents the outcome of a single assertion evaluation.
/// </summary>
public sealed class AssertionEvent
{
	/// <summary>Whether the assertion passed.</summary>
	public bool Passed { get; }

	/// <summary>The name of the assertion method (e.g. "Is", "IsGreaterThan").</summary>
	public string? Assertion { get; }

	/// <summary>The actual value that was asserted.</summary>
	public object? Actual { get; }

	/// <summary>The expected value, if applicable.</summary>
	public object? Expected { get; }

	/// <summary>The caller's source file, if available.</summary>
	public string? File { get; }

	/// <summary>The caller's line number, if available.</summary>
	public int? Line { get; }

	/// <summary>The source code line content, if available.</summary>
	public string? Code { get; }

	/// <summary>The timestamp when the assertion was evaluated.</summary>
	public DateTime Timestamp { get; } = DateTime.Now;

	/// <summary>The associated <see cref="Failure"/>, if the assertion failed.</summary>
	public Failure? Failure { get; }

	internal AssertionEvent(bool passed, Failure? failure = null)
	{
		Passed = passed;
		Failure = failure;

		if (failure is not null)
		{
			Assertion = failure.Assertion;
			Actual = failure.Actual;
			Expected = failure.Expected;
			File = failure.File;
			Line = failure.Line;
		}
	}

	internal AssertionEvent(bool passed, string? assertion, string? file, int? line, string? code = null)
	{
		Passed = passed;
		Assertion = assertion;
		File = file;
		Line = line;
		Code = code;
	}
}
