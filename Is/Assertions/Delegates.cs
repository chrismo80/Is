﻿using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Delegates
{
	/// <summary>
	/// Asserts that the given <paramref name="action" /> throws
	/// an exception of type <typeparamref name="T" />.
	/// </summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static T? IsThrowing<T>(this Action action) where T : Exception =>
		action.CatchException()?.Is<T>() ?? Assertion.Failed<T>(typeof(T), "is not thrown");

	/// <summary>
	/// Asserts that the given <paramref name="action" /> does not throw
	/// an exception of type <typeparamref name="T" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotThrowing<T>(this Action action) where T : Exception =>
		action.CatchException()?.IsNot<T>() ?? Assertion.Passed();

	/// <summary>
	/// Asserts that the given synchronous <paramref name="action"/> throws
	/// an exception of type <typeparamref name="T"/>
	/// and that the exception message contains
	/// the specified <paramref name="message"/> substring.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsThrowing<T>(this Action action, string message) where T : Exception =>
		action.IsThrowing<T>()?.Message.IsContaining(message) ?? false;

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> throws
	/// an exception of type <typeparamref name="T" />.
	/// </summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static T? IsThrowing<T>(this Func<Task> function) where T : Exception =>
		function.ToAction().IsThrowing<T>();

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> does not throw
	/// an exception of type <typeparamref name="T" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsNotThrowing<T>(this Func<Task> function) where T : Exception =>
		function.ToAction().IsNotThrowing<T>();

	/// <summary>
	/// Asserts that the given asynchronous <paramref name="function"/> throws
	/// an exception of type <typeparamref name="T"/>
	/// and that the exception message contains
	/// the specified <paramref name="message"/> substring.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsThrowing<T>(this Func<Task> function, string message) where T : Exception =>
		function.IsThrowing<T>()?.Message.IsContaining(message) ?? false;

	/// <summary>
	/// Asserts that the given <paramref name="action" /> did complete
	/// within a specific <paramref name="timespan" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Action action, TimeSpan timespan) => Check
		.That(Task.Run(action).Wait(timespan))
		.Unless(action, "is not completing within", timespan);

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> did complete
	/// within a specific <paramref name="timespan" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsCompletingWithin(this Func<Task> function, TimeSpan timespan) =>
		function.ToAction().IsCompletingWithin(timespan);

	/// <summary>
	/// Asserts that the given <paramref name="action" /> is allocating
	/// not more than <paramref name="kiloBytes" />.
	/// </summary>
	public static bool IsAllocatingAtMost(this Action action, long kiloBytes)
	{
		long before = GC.GetTotalMemory(true);
		action();
		long after = GC.GetTotalMemory(false);

		long allocated = (after - before) / 1024;

		return allocated.IsAtMost(kiloBytes);
	}

	/// <summary>
	/// Asserts that the given async <paramref name="function" /> is allocating
	/// not more than <paramref name="kiloBytes" />.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsAllocatingAtMost(this Func<Task> function, long kiloBytes) =>
		function.ToAction().IsAllocatingAtMost(kiloBytes);

	private static Action ToAction(this Func<Task> function) =>
		() => function().GetAwaiter().GetResult();

	private static Exception? CatchException(this Action action)
	{
		try { action(); }
		catch (Exception ex) { return ex; }
		return null;
	}
}