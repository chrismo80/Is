using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace Is.Tests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AssertionContextAttribute : NUnitAttribute, IWrapTestMethod
{
	public TestCommand Wrap(TestCommand command) => new AssertionContextCommand(command);

	private sealed class AssertionContextCommand(TestCommand innerCommand) : DelegatingTestCommand(innerCommand)
	{
		public override TestResult Execute(TestExecutionContext testContext)
		{
			var caller = testContext.CurrentTest.Method?.MethodInfo.Name ?? testContext.CurrentTest.Name;

			using var assertionContext = AssertionContext.Begin(caller);

			return innerCommand.Execute(testContext);
		}
	}
}