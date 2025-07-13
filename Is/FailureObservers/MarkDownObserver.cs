using Is.Core;
using Is.Core.Interfaces;
using Is.Tools;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace Is.FailureObservers;

/// <summary>
/// <see cref="IFailureObserver"/> that writes all failures into
/// one FailureReport mark-down file.
/// </summary>
[DebuggerStepThrough]
public class MarkDownObserver : IFailureObserver
{
	private const string FILENAME = "FailureReport.md";
	private static readonly object sync = new();

	static MarkDownObserver() => File.Delete(FILENAME);

	public void OnFailure(Failure failure)
	{
		lock (sync)
		{
			File.AppendAllText(FILENAME, failure.ToMarkDown());
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
		sb.AppendLine($"**Time:** `{failure.Created}`");
		sb.AppendLine();
		sb.AppendLine($"**Code:** `{failure.File}` line `{failure.Line}`");
		sb.AppendLine($"```csharp");
		sb.AppendLine($"{failure.Code}");
		sb.AppendLine($"```");
		sb.AppendLine();
		sb.AppendLine("## 📋 Message");
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
			sb.AppendLine($"| `{sub.Message.RemoveColor().Path().Beautify()}` | {sub.Values()} |");

		return sb.AppendLine().AppendLine().ToString();
	}

	private static string Path(this string message) =>
		message.Split(": ").First();

	private static string Beautify(this string text) =>
		text.Replace(".", "`__.__`");

	private static string Value(this object? value) =>
		value is null ? "__---__" : $"`{value}`";

	private static string Values(this Failure failure) =>
		failure.Actual is null || failure.Expected is null || failure.Actual?.GetType() == failure.Expected?.GetType() ?
			$"{failure.Actual.Value()} | {failure.Expected.Value()}" :
			$"{failure.Actual.Value()} ({failure.Actual?.GetType().Name}) | {failure.Expected.Value()} ({failure.Expected?.GetType().Name})";
}