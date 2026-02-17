using Is.Core;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;

namespace Is.Assertions;

[DebuggerStepThrough]
public static class Files
{
	/// <summary>
	/// Asserts that the specified <paramref name="path"/> exists as a file or directory.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsExisting(this string path,
		[CallerArgumentExpression("path")] string? expression = null) => Check
		.That(File.Exists(path) || Directory.Exists(path))
		.Unless(path, "does not exist", expression);
}
