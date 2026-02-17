# AGENTS.md - Is Assertion Library

This document provides guidance for AI coding agents working in this repository.

## Project Overview

`Is` is a lightweight .NET assertion library focusing on readable, minimal, and fail-fast test expectations. It uses extension methods for a fluent API (e.g., `value.Is(expected)`).

## Build/Lint/Test Commands

### Build
```bash
dotnet build
```

### Run all Tests
```bash
dotnet test
```

### Run Single Test (by name)
```bash
dotnet test --filter "FullyQualifiedName~ComparisonTests.IsGreaterThan_IsSmallerThan"
```

### Run Tests in Specific Class
```bash
dotnet test --filter "FullyQualifiedName~ComparisonTests"
```

### Pack NuGet Package
```bash
dotnet pack
```

## Project Structure

```
Is/Assertions/     # All assertion extension methods (Comparisons.cs, Equality.cs, etc.)
Is/Core/           # Core infrastructure (Check.cs, Assertion.cs, Configuration.cs)
Is/Tools/          # Internal utilities (MessageExtensions.cs, etc.)
Is.Tests/          # Test project (NUnit)
```

## Code Style Guidelines

### Naming Conventions

- **Public methods**: Start with `Is` (e.g., `IsGreaterThan`, `IsNotEmpty`)
- **Parameters**: Use `actual` for tested value, `expected` for comparison, `other` for alternatives
- **Expression parameter**: Always `expression` with `[CallerArgumentExpression("actual")]`

### Imports Order
1. System namespaces
2. Third-party namespaces
3. Project namespaces (`Is.Core`, `Is.Tools`)

### Formatting

- **Indentation**: Tabs (not spaces)
- **Braces**: Allman style (opening brace on new line)
- **Method attributes**: Place on separate lines above the method

### Method Structure Pattern

```csharp
/// <summary>
/// XML documentation explaining the assertion.
/// </summary>
[MethodImpl(MethodImplOptions.NoInlining)]
public static bool IsSomething<T>(this T actual, T other,
    [CallerArgumentExpression("actual")] string? expression = null) where T : IComparable<T> => Check
    .That(actual.CompareTo(other) > 0)
    .Unless(actual, "is not something", other, expression);
```

### Expression Parameter Pattern

1. Add: `[CallerArgumentExpression("actual")] string? expression = null`
2. Pass to `.Unless()`: `.Unless(actual, "message", other, expression)`
3. `WithExpression()` formats as: `[expression] = message`

### Types

- **Target Framework**: .NET 8.0
- **Nullable**: Enabled
- **Implicit Usings**: Enabled
- Use generic constraints: `where T : IComparable<T>`, `where T : INumber<T>`

### Error Messages

- Use lowercase: `"is not greater than"`, `"is empty"`
- Format: `actual (Type) is not expected (Type)`

### Switch Expressions

```csharp
return condition switch
{
    true => Assertion.Passed(result),
    false => Assertion.Failed<TResult>(message)
};
```

### DebuggerStepThrough

Apply to all public static assertion classes and internal structs.

### Testing Conventions

- Framework: NUnit with `[TestFixture]`
- Configure adapter in `[SetUp]` and `[TearDown]`
- Use `[TestCase]` for parameterized tests

```csharp
[TestFixture]
public class MyTests
{
    [SetUp]
    public void SetUp() => Configuration.Active.TestAdapter = new DefaultAdapter();

    [TearDown]
    public void TearDown() => Configuration.Active.TestAdapter = new DefaultAdapter();

    [Test]
    [TestCase(3, 4)]
    [TestCase(5.0, 9.0)]
    public void GenericTest<T>(T actual, T expected) where T : IComparable<T>
    {
        expected.IsGreaterThan(actual);
    }
}
```

### When Adding New Assertions

1. Add method in `Is/Assertions/`
2. Follow naming: `Is{Condition}`
3. Add `[CallerArgumentExpression]` parameter
4. Use `Check.That().Unless()` pattern
5. Add XML documentation
6. Create tests in `Is.Tests/Assertions/`
7. Update `Docs/Is.md`

### Avoid

- NO inline code comments
- NO `var` when type is not obvious
- NO catching exceptions in assertions
- NO new dependencies without discussion
