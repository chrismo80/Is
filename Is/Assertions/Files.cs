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
	public static bool IsExisting(this string path) => Check
		.That(File.Exists(path) || Directory.Exists(path))
		.Unless(path, "does not exist");

	/// <summary>
	/// Asserts that the specified <paramref name="path"/> exists and is a file.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsFile(this string path) => Check
		.That(File.Exists(path))
		.Unless(path, "is not a file");

	/// <summary>
	/// Asserts that the specified <paramref name="path"/> exists and is a directory.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool IsDirectory(this string path) => Check
		.That(Directory.Exists(path))
		.Unless(path, "is not a directory");
}
