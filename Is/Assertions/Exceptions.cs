namespace Is.Assertions;

public static class Exceptions
{
	/// <summary>Asserts that the given <paramref name="action" /> throws an exception of type <typeparamref name="T" />.</summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	public static T IsThrowing<T>(this Action action) where T : Exception
	{
		try { action(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new NotException(typeof(T), "was not thrown");
	}

	/// <summary>Asserts that the given async <paramref name="action" /> throws an exception of type <typeparamref name="T" />.</summary>
	/// <returns>The thrown exception of type <typeparamref name="T" />.</returns>
	public static T IsThrowing<T>(this Func<Task> action) where T : Exception
	{
		try { action().GetAwaiter().GetResult(); }
		catch (Exception ex) { return ex.Is<T>(); }

		throw new NotException(typeof(T), "was not thrown");
	}

	/// <summary>Asserts that the given synchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/> and that the exception message contains the specified <paramref name="message"/> substring.</summary>
	public static bool IsThrowing<T>(this Action action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);

	/// <summary>Asserts that the given asynchronous <paramref name="action"/> throws an exception of type <typeparamref name="T"/> and that the exception message contains the specified <paramref name="message"/> substring.</summary>
	public static bool IsThrowing<T>(this Func<Task> action, string message) where T : Exception =>
		action.IsThrowing<T>().Message.IsContaining(message);
}