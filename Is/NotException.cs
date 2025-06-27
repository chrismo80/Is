using Is.Core;
using System.Diagnostics;

namespace Is;

[DebuggerStepThrough]
public class NotException(Failure failure) : Exception(failure.Message);