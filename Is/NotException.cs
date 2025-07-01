using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public class NotException(string message) : Exception(message);