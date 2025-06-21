using Is.Core;
using System.Reflection;

namespace Is.Tests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AssertionContextAttribute
	: NUnitAttribute, NUnit.Framework.Interfaces.IWrapTestMethod
{
	public NUnit.Framework.Internal.Commands.TestCommand Wrap(NUnit.Framework.Internal.Commands.TestCommand command) =>
		new AssertionContextCommand(command);

	private sealed class AssertionContextCommand(NUnit.Framework.Internal.Commands.TestCommand innerCommand)
		: NUnit.Framework.Internal.Commands.DelegatingTestCommand(innerCommand)
	{
		public override NUnit.Framework.Internal.TestResult Execute(NUnit.Framework.Internal.TestExecutionContext testContext)
		{
			using var assertionContext = AssertionContext.Begin(Caller(testContext));

			return innerCommand.Execute(testContext);
		}

		private static string Caller(NUnit.Framework.Internal.TestExecutionContext testContext) =>
			Info(testContext.CurrentTest.Method?.MethodInfo);

		private static string Info(MethodInfo? method) =>
			$"{method?.DeclaringType?.Name}.{method?.Name}";
	}
}