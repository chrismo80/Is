using Is.Core;
using System.Diagnostics;

namespace Is.Tools;

[DebuggerStepThrough]
internal static class TypeExtensions
{
	internal static Type? ToType(this string typeName) =>
		Type.GetType(typeName, false);

	internal static T? ToInstance<T>(this Type type) =>
		(T?)type.GetConstructor(Type.EmptyTypes)?.Invoke(null);

	internal static T? ToInstance<T>(this Type type, string message) =>
		(T?)type.GetConstructor([typeof(string)])?.Invoke([message]);

	internal static Exception ToException(this string message, Type type) =>
		type.ToInstance<Exception>(message) ?? new NotException(message);

	internal static Exception ToException(this Failure failure) =>
		failure.CustomExceptionType?.ToInstance<Exception>(failure.Message) ?? new NotException(failure.Message);
}