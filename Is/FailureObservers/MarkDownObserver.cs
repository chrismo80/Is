using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;
using System.Text;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IAssertionObserver"/> that writes all assertions into
/// one FailureReport mark-down file with statistics.
/// </summary>
[DebuggerStepThrough]
public class MarkDownObserver : IAssertionObserver, IDisposable
{
	private const string FILENAME = "FailureReport.md";
	private static readonly object sync = new();
	private static int totalCount;
	private static int passedCount;
	private static int failedCount;

	static MarkDownObserver() => Reset();

	public void OnAssertion(AssertionEvent assertionEvent)
	{
		lock (sync)
		{
			totalCount++;
			if (assertionEvent.Passed)
				passedCount++;
			else
				failedCount++;

			if (!assertionEvent.Passed)
			{
				File.AppendAllText(FILENAME, assertionEvent.ToMarkDown());
			}
		}
	}

	/// <summary>
	/// Writes the statistics summary to the beginning of the file.
	/// Call this when done (e.g., at test suite end).
	/// </summary>
	public void Dispose()
	{
		WriteSummary();
	}

	private static void WriteSummary()
	{
		lock (sync)
		{
			if (totalCount == 0)
				return;

			var sb = new StringBuilder();
			sb.AppendLine("# 📊 Assertion Statistics");
			sb.AppendLine();
			sb.AppendLine($"- **Total:** {totalCount} assertions");
			sb.AppendLine($"- **Passed:** {passedCount} ({GetPercentage(passedCount)}%)");
			sb.AppendLine($"- **Failed:** {failedCount} ({GetPercentage(failedCount)}%)");
			sb.AppendLine();
			sb.AppendLine("---");
			sb.AppendLine();

			var existing = File.Exists(FILENAME) ? File.ReadAllText(FILENAME) : "";
			File.WriteAllText(FILENAME, sb.ToString() + existing);
		}
	}

	private static double GetPercentage(int count) =>
		totalCount > 0 ? Math.Round(count * 100.0 / totalCount, 1) : 0;

	/// <summary>Resets statistics. Called automatically when a new observer is created.</summary>
	public static void Reset()
	{
		lock (sync)
		{
			totalCount = 0;
			passedCount = 0;
			failedCount = 0;
			File.Delete(FILENAME);
		}
	}
}

[DebuggerStepThrough]
file static class MarkDownExtensions
{
	internal static string ToMarkDown(this AssertionEvent assertionEvent)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"# ❌ Assertion failed: `{assertionEvent.Assertion}`");
		sb.AppendLine();
		sb.AppendLine($"**Time:** `{assertionEvent.Timestamp}`");
		sb.AppendLine();
		sb.AppendLine($"**Code:** `{assertionEvent.File}` line `{assertionEvent.Line}`");
		sb.AppendLine($"```csharp");
		sb.AppendLine($"{assertionEvent.Code}");
		sb.AppendLine($"```");
		sb.AppendLine();
		sb.AppendLine("## 📋 Message");
		sb.AppendLine();

		if (assertionEvent.InnerEvents == null)
			sb.Append('\t');

		sb.AppendLine(assertionEvent.Message?.RemoveColor().Trim() ?? "No message");

		if (assertionEvent.InnerEvents == null)
			return sb.AppendLine().ToString();

		sb.AppendLine();
		sb.AppendLine("## 🔍 Details");
		sb.AppendLine();
		sb.AppendLine("| Path | Actual | Expected |");
		sb.AppendLine("|-----|--------|---------|");

		foreach (var sub in assertionEvent.InnerEvents)
			sb.AppendLine($"| `{sub.Message?.RemoveColor().Path().Beautify()}` | {sub.Values()} |");

		return sb.AppendLine().AppendLine().ToString();
	}

	private static string Path(this string message) =>
		message.Split(": ").First();

	private static string Beautify(this string text) =>
		text.Replace(".", "`__.__`");

	private static string Value(this object? value) =>
		value is null ? "__---__" : $"`{value}`";

	private static string Values(this AssertionEvent assertionEvent) =>
		assertionEvent.Actual is null || assertionEvent.Expected is null || assertionEvent.Actual?.GetType() == assertionEvent.Expected?.GetType() ?
			$"{assertionEvent.Actual.Value()} | {assertionEvent.Expected.Value()}" :
			$"{assertionEvent.Actual.Value()} ({assertionEvent.Actual?.GetType().Name}) | {assertionEvent.Expected.Value()} ({assertionEvent.Expected?.GetType().Name})";
}
