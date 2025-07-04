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
	private static readonly object sync = new();

	public string Filename { get; set; } = "FailureReport.md";

	public MarkDownObserver() => File.Delete(Filename);

	public void OnFailure(Failure failure)
	{
		lock (sync)
		{
			File.AppendAllText(Filename, failure.ToMarkDown());
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
		sb.AppendLine("| Path | Actual | Expected |");
		sb.AppendLine("|-----|--------|---------|");

		foreach (var sub in failure.Failures)
			sb.AppendLine($"| `{sub.Message.RemoveColor().Path().Beautify()}` | {sub.Actual.Value()} | {sub.Expected.Value()} |");

		return sb.AppendLine().AppendLine().ToString();
	}

	private static string Beautify(this string text) =>
		text.Replace(".", "`__.__`");

	private static string Path(this string message) =>
		message.Split(": ").First();

	private static string Value(this object? value) =>
		value is null ? "__---__" : $"`{value}`";
}