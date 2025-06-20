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
			var caller = testContext.CurrentTest.Method?.MethodInfo.Name ?? testContext.CurrentTest.Name;

			using var assertionContext = AssertionContext.Begin(caller);

			return innerCommand.Execute(testContext);
		}
	}
}