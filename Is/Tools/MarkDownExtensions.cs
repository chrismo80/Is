using Is.Core;
using System.Diagnostics;
using System.Text;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class MarkDownExtensions
{
	internal static string ToMarkDown(this Failure failure)
	{
		var sb = new StringBuilder();

		sb.AppendLine($"# ❌ Assertion failed: `{failure.Assertion}`");
		sb.AppendLine();
		sb.AppendLine($"**Location:** `{failure.File}` line {failure.Line}");
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