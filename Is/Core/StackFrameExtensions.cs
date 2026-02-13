using System.Diagnostics;
using System.Reflection;

namespace Is.Core;

[DebuggerStepThrough]
internal static class StackFrameExtensions
{
	private static readonly Assembly Mine = Assembly.GetExecutingAssembly();

	internal static StackFrame? FindFrame(this StackFrame[] frames) =>
		frames.FirstOrDefault(f => f.IsExtensionCall() && f.GetFileName() != null);

	private static bool IsExtensionCall(this StackFrame frame) =>
		(frame.GetMethod()?.IsForeignAssembly() ?? false) && (frame.GetMethod()?.HasNotAttribute() ?? false);

	private static bool IsForeignAssembly(this MethodBase method) =>
		method.DeclaringType?.Assembly != Mine && !Attribute.IsDefined(method, typeof(IsAssertionAttribute));

	private static bool HasNotAttribute(this MethodBase method) =>
		!Attribute.IsDefined(method, typeof(IsAssertionAttribute)) && !Attribute.IsDefined(method.DeclaringType, typeof(IsAssertionsAttribute));
}
