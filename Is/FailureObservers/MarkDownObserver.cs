using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;
using System.Text;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IFailureObserver"/> that writes all failures into
/// one FailureReport mark-down file.
/// </summary>
[DebuggerStepThrough]
public class MarkDownObserver : IFailureObserver
{
	const string FILE = "FailureReport.md";

	private static readonly object sync = new();

	static MarkDownObserver() => File.Delete(FILE);

	public void OnFailure(Failure failure)
	{
		lock (sync)
		{
			File.AppendAllText(FILE, failure.ToMarkDown());
		}
	}
}

[DebuggerStepThrough]
file static class MarkDownExtensions
{
	internal static string ToMarkDown(this Failure failure)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"# ❌ Assertion failed: `{failure.Assertion}`");
		sb.AppendLine();
		sb.AppendLine($"**Location:** `{failure.File}` line {failure.Line}");
		sb.AppendLine();
		sb.AppendLine($"**Code:** `{failure.Code}`");
		sb.AppendLine();
		sb.AppendLine($"**Time:** `{failure.Created}`");
		sb.AppendLine();
		sb.AppendLine("## 📋 Summary");
		sb.AppendLine();

		if (failure.Failures == null)
			sb.Append('\t');

		sb.AppendLine(failure.Message.RemoveColor().Trim());

		if (failure.Failures == null)
			return sb.AppendLine().ToString();

		sb.AppendLine();
		sb.AppendLine("## 🔍 Details");
		sb.AppendLine();
		sb.AppendLine("| Path | Message | Actual | Expected |");
		sb.AppendLine("|-----|---------|--------|---------|");

		foreach (var sub in failure.Failures)
			sb.AppendLine($"| `{sub.Message.RemoveColor().Path()}` | {EscapeMarkdown(sub.Message.RemoveColor().Text())} | `{sub.Actual}` | `{sub.Expected}` |");

		return sb.AppendLine().ToString();
	}

	private static string EscapeMarkdown(string text) =>
		text.Replace("|", "\\|");

	private static string Path(this string message) =>
		message.Split(":").First();

	private static string Text(this string message) =>
		message.Split(":").Last();
}