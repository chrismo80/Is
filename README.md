![plot](Is.png)
# Minimalistic Assertion Extensions for .NET

**Simple. Readable. Opinionated.**

`Is` is a lightweight assertion library for .NET that focuses on _readable_, _minimal_, and _fail-fast_ test expectations — no assertion clutter, no dependencies, no test framework lock-in.


## ✅ Why use Is?

- 📘 Concise: One word. One assertion.
- 💥 Opinionated: Less is more. Only core assertions relevant for real-world use cases, just fast failure and clarity.
- 🧪 Test-framework agnostic: Works with xUnit, NUnit, MSTest, or none at all.
- ⚙️ Self-contained: No dependencies, no configuration, just drop it in.
- 🔧 Useful for unit tests, guard clauses or other validation checks

## 📦 Get It on NuGet

[![NuGet](https://img.shields.io/nuget/v/AssertWithIs.svg)](https://www.nuget.org/packages/AssertWithIs/)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://www.nuget.org/packages/AssertWithIs/)

The package is published on NuGet under the name [`AssertWithIs`](https://www.nuget.org/packages/AssertWithIs/) because shorter IDs like `Is` or `Is.Assertions` were already taken or reserved.  
Despite the package name, the library itself uses the concise `Is` namespace and generates a single `Is.dll`, so your code stays clean and expressive:


## 🔍 Available Methods

All public methods in `Is` are:

- ❗️ **Extension methods**, designed to be used fluently (`value.Is(...)`)
- 🔤 **Named consistently**: Every method starts with `Is`, making them easy to discover with IntelliSense
- ✂️ **Minimal and deliberate**: Only a small, opinionated set of assertions is exposed

> ✅ Because all methods start with `Is`, you can type `.` and just filter by `Is` in IntelliSense. Fast and frictionless.

The full public API and its extension methods can be [found here](https://github.com/chrismo80/Is/blob/main/Is.md)


## 🔧 Usage Examples

### Basic value checks
```csharp
42.Is(42);       // ✅ passes
42.Is(41);       // ❌ throws Is.NotException: 42 (System.Int32) is not 41 (System.Int32)
42.Is(42.0);     // ❌ throws Is.NotException: 42 (System.Int32) is not 42 (System.Double)

"test".Is("test");               // ✅ passes
```

### Collection checks
```csharp
new[] { 1, 2, 3 }.Is(1, 2, 3);   // ✅ passes (enumerable values check)

new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);     // ✅ passes
new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);        // ✅ passes
new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);           // ✅ passes

new List<int> { 1, 2, 3, 4 }.IsContaining(1, 2);    // ✅ passes
new List<int> { 1, 2 }.IsIn(1, 2, 3, 4);            // ✅ passes
```

### Type checks
```csharp
"hello".Is<string>();     // ✅ passes
"hello".Is<int>();        // ❌ throws Is.NotException: "hello" (System.String) is no System.Int32
```

### Numeric comparisons
```csharp
2.999999f.Is(3f)         // ✅ passes
783.0123.Is(783.0124)    // ✅ passes

5.IsSmallerThan(6);      // ✅ passes
6.IsGreaterThan(5.0);    // ✅ passes
5.IsGreaterThan(6);      // ❌ throws Is.NotException: 5 (System.Int32) is not greater than 6 (System.Int32)
2.IsBetween(1, 3);       // ✅ passes

(0.1 + 0.2).Is(0.3);                // ✅ passes
(0.1 + 0.2).IsExactly(0.3);         // ❌ fails
(0.1 + 0.2).IsApproximately(0.3);   // ✅ passes

(1.0 / 3.0).Is(0.333333);     // ✅ passes
(1.0 / 3.0).Is(0.33333);      // ❌ throws Is.NotException: 0,33333 (System.Double) is not close to 0,3333333333333333 (System.Double)
```

### Exception assertions
```csharp
static int DivideByZero(int value) => value / 0;
Action action = () => _ = DivideByZero(1);
action.IsThrowing<DivideByZeroException>();  // ✅ passes

Action action = () => 5.IsGreaterThan(6);
action.IsThrowing<Is.NotException>("is not greater than");    // ✅ passes
```

### String checks
```csharp
var groups = "hello world".IsMatching("(.*) (.*)");  // ✅ passes
groups[1].Value.Is("hello");  // ✅ passes
groups[2].Value.Is("world");  // ✅ passes

"hello world".IsContaining("hello");    // ✅ passes
```


## ❌ Error messages

Exception messages
- uses colors to highlight important parts
- displays the source of the error (line number and code)

![plot](Docs/SimpleErrorMessage.png)

![plot](Docs/ComplexErrorMessage.png)

## ⚖️ Design Philosophy: Clarity over Chaining

- ❌ No ```.Should()```, no fluent bloat
- ✅ Focus on positive assertions
- 📢 Failure messages like: ```42 (System.Int32) is not 41 (System.Int32)```
- 🧠 Designed to make tests read like intentions, not machinery

❌ Avoid Chaining

While fluent-style chaining such as:

```csharp
value
    .IsPositive()
    .IsGreaterThan(6)
    .IsBetween(6, 12);
```
can look elegant, it introduces trade-offs that conflict with design goals:

- 🧩 Supporting both chaining and boolean-returning methods would mean duplicating logic, making the library harder to maintain.
- 🔄 Useful patterns like .All(x => x.IsPositive()) require boolean-returning extensions — chaining breaks this.
- 📏 Chaining implies stateful assertion objects; this library favors stateless, minimal assertions for predictability and simplicity.
- ✅ Recommended calling assertions directly and explicitly:

```csharp
value.IsPositive();
value.IsGreaterThan(6);
value.IsBetween(6, 12);
```
Enables collection assertion like:
```csharp
list.All(item => item.IsPositive());
```


## ⚙️ Configuration: Enable/Disable Exception Throwing

The `Is` library allows users to control whether assertion failures throw exceptions or not.
By default, assertion failures throw a `NotException`.
However, you can modify this behavior using the `Configuration.ThrowOnFailure` flag.
If disabled, assertions will instead return `false` on failure and log the exception message using the configured logger.

```csharp
Configuration.Logger = Console.WriteLine;

Configuration.ThrowOnFailure = false;

3.Is(4); // ❌

Configuration.ThrowOnFailure = true;
```
    
### Key Properties
- **`ThrowOnFailure`**: A `bool` indicating whether assertions throw exceptions on failure. Default is `true`.
- **`Logger`**: An optional delegate to handle log messages when exceptions are disabled. Defaults to writing messages to `System.Diagnostics.Debug.WriteLine`.

## 🔄 Grouped Assertion Evaluation with AssertionContext

Sometimes you want to run multiple assertions in a test and evaluate all failures at once, rather than stopping after the first one. The AssertionContext provides exactly that capability.

```csharp
using var context = AssertionContext.Begin();

false.IsTrue();       // ❌ fails
4.Is(5);              // ❌ fails

context.FailureCount.Is(2);

// You can inspect failures manually:
context.NextFailure().Message.IsContaining("false.IsTrue()");
context.NextFailure().Message.IsContaining("4.Is(5)");
```

If any assertion failures remain unhandled when the context is disposed, an AggregateException is thrown containing all captured NotExceptions:

```csharp
try
{
    using var context = AssertionContext.Begin();

    "abc".IsContaining("xyz"); // ❌
    42.Is(0);                  // ❌
}
catch (AggregateException ex)
{
    ex.InnerExceptions.Count.Is(2);
}
```


🔒 Scoped Context:

Only one context can be active per async-flow at a time. It uses AsyncLocal<T> for full async test compatibility.

🧪 Designed for Integration:

Works with NUnit, xUnit, or MSTest, either manually via using or with custom test base classes or attributes.
To keep the package dependency-free, such implementations are out of scope for the library, but here is an example for such an Attribute for NUnit.

```csharp
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
```

This allows you to verify NotException like this:

```csharp
[Test]
[AssertionContext]
public void ContextTest_WithAttribute()
{
    false.IsTrue();
    4.Is(5);

    AssertionContext.Current?.NextFailure();
    AssertionContext.Current?.NextFailure();
}
```





## 🔧 Custom Assertions
Create a static class with an extension method that performs the desired assertion.
Use the built-in **`Check`** fluent API to insert the assertion into the features of the library, such as AssertionContext and message formatting.

```csharp
public static class CustomAssertions
{
	[IsExtension]
	public static bool IsCustomAssertion(this int value, [CallerArgumentExpression("value")] string? expr = null) =>
		Check.That(value > 0).Unless(value, $"in '{expr}' is not positive");
}
```

✅ Usage Example

```csharp
(9 - 5).IsCustomAssertion(); // ✅
(5 - 9).IsCustomAssertion(); // ❌
```
Results in the following error message:

```
Is.NotException :

	-4 (Int32)

	in '5 - 9' is not positive

in Is.Tests.Assertions in line 639: (5 - 9).IsCustomAssertion();
```

ℹ️ Your custom assertions integrate seamlessly with the existing fluent style of the library.







## 🔍 Key Advantages of Is

- 🧠 Ultra-Concise Syntax with Natural Readability
- 🧵 Minimal Dependencies / Fast Startup
    - Lean and dependency-free — ideal for CI pipelines or constrained environments.
- 🧪 Focused on Behavior, Not Chaining
    - Prioritizes clarity over fluent DSL chaining.
- 🔧 Extensible and Easy to Maintain
    - Simple to audit, fork, and adapt for your team or test infrastructure.








## 📝 License

MIT – use freely.

## 🙌 Contributing

Ideas, bug reports, or pull requests are always welcome.

## ❤️ Author

Developed with care by chrismo80