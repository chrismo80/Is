using Is.Core;
using Is.Core.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Is.AssertionListeners;

/// <summary>
/// <see cref="IAssertionListener"/> that collects statistics about all assertion evaluations.
/// Thread-safe. Query <see cref="Total"/>, <see cref="TotalPassed"/>, <see cref="TotalFailed"/> 
/// and <see cref="Summary()"/> at any time.
/// </summary>
[DebuggerStepThrough]
public class StatisticsListener : IAssertionListener
{
	private readonly ConcurrentDictionary<string, AssertionStats> _stats = new();
	private int _totalPassed;
	private int _totalFailed;

	/// <summary>Total number of assertions evaluated.</summary>
	public int Total => _totalPassed + _totalFailed;

	/// <summary>Total number of passed assertions.</summary>
	public int TotalPassed => _totalPassed;

	/// <summary>Total number of failed assertions.</summary>
	public int TotalFailed => _totalFailed;

	/// <summary>Pass rate as a value between 0.0 and 1.0.</summary>
	public double PassRate => Total == 0 ? 1.0 : (double)_totalPassed / Total;

	/// <summary>Statistics per assertion type.</summary>
	public IReadOnlyDictionary<string, AssertionStats> PerAssertion => _stats;

	public void OnAssertion(AssertionEvent e)
	{
		if (e.Passed)
			Interlocked.Increment(ref _totalPassed);
		else
			Interlocked.Increment(ref _totalFailed);

		var key = e.Assertion ?? "Unknown";
		_stats.AddOrUpdate(key,
			_ => new AssertionStats(e.Passed),
			(_, existing) => existing.Record(e.Passed));
	}

	/// <summary>
	/// Returns a formatted summary of all assertion statistics.
	/// </summary>
	public string Summary()
	{
		var sb = new StringBuilder();

		sb.AppendLine($"Assertions: {Total} total, {TotalPassed} passed, {TotalFailed} failed ({PassRate:P1})");
		sb.AppendLine();

		foreach (var (name, stats) in _stats.OrderByDescending(x => x.Value.Total))
			sb.AppendLine($"  {name,-30} {stats.Passed,5} passed  {stats.Failed,5} failed");

		return sb.ToString();
	}

	/// <summary>Resets all collected statistics.</summary>
	public void Reset()
	{
		_stats.Clear();
		Interlocked.Exchange(ref _totalPassed, 0);
		Interlocked.Exchange(ref _totalFailed, 0);
	}
}

/// <summary>
/// Pass/fail counters for a single assertion type.
/// </summary>
public sealed class AssertionStats
{
	/// <summary>Number of times this assertion passed.</summary>
	public int Passed { get; }

	/// <summary>Number of times this assertion failed.</summary>
	public int Failed { get; }

	/// <summary>Total evaluations of this assertion.</summary>
	public int Total => Passed + Failed;

	internal AssertionStats(bool passed)
	{
		Passed = passed ? 1 : 0;
		Failed = passed ? 0 : 1;
	}

	private AssertionStats(int passed, int failed)
	{
		Passed = passed;
		Failed = failed;
	}

	internal AssertionStats Record(bool pass) =>
		new(Passed + (pass ? 1 : 0), Failed + (pass ? 0 : 1));
}
