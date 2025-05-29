# 📦 Is – Minimalistic Assertion Extensions for .NET

**Simple. Readable. Opinionated.**

`Is` is a lightweight assertion library for .NET that focuses on _readable_, _minimal_, and _fail-fast_ test expectations — no assertion clutter, no dependencies, no test framework lock-in.


## ✅ Why use Is?

- 📘 Concise: One word. One assertion.
- 💥 Opinionated: No "IsNot", only assert what you expect, not what you not expect, just fast failure and clarity.
- 🧪 Test-framework agnostic: Works with xUnit, NUnit, MSTest, or none at all.
- ⚙️ Self-contained: No dependencies, no configuration, just drop it in.




## 🔍 Available Methods

All public methods in `Is` are:

- ❗️ **Extension methods**, designed to be used fluently (`value.Is(...)`)
- 🔤 **Named consistently**: Every method starts with `Is`, making them easy to discover with IntelliSense
- ✂️ **Minimal and deliberate**: Only a small, opinionated set of assertions is exposed


| Method | Description |
|------------------|-------------|
| `IsThrowing<T>()` | Asserts that the given synchronous action throws an exception of type `T`. Returns the exception. |
| `IsThrowing<T>(message)` | Asserts that the synchronous action throws an exception of type `T` and that the message contains the specified substring. |
| `IsThrowing<T>()` | Asserts that the given asynchronous function throws an exception of type `T`. Returns the exception. |
| `IsThrowing<T>(message)` | Asserts that the async function throws an exception of type `T` and the message contains the given substring. |
| `Is<T>()` | Asserts that the object is of type `T` and returns its cast. |
| `Is(expected)` | Asserts that the object matches one or more expected values. |
| `IsExactly(expected)` | Asserts that the object is exactly equal to the expected value. |
| `IsEmpty()` | Asserts that the sequence is empty. |
| `IsGreaterThan(other)` | Asserts that `actual` is greater than `other`. |
| `IsSmallerThan(other)` | Asserts that `actual` is smaller than `other`. |
| `IsContaining(values)` | Asserts that the sequence contains all specified values. |
| `IsContaining(substring)` | Asserts that the string contains the specified substring. |
| `IsMatching(pattern)` | Asserts that the string matches the given regular expression pattern. Returns the match groups. |
| `IsIn(values)` | Asserts that all elements in `actual` exist in the provided values. |
| `IsBetween(min, max)` | Asserts that `actual` is strictly between `min` and `max`. |
| `IsApproximately(other, epsilon)` | Checks if two values are approximately equal within a specified tolerance. |
| `IsNull()` | Asserts that the object is `null`. |
| `IsTrue()` | Asserts that a boolean is `true`. |
| `IsFalse()` | Asserts that a boolean is `false`. |





> ✅ Because all methods start with `Is`, you can type `.` and just filter by `Is` in IntelliSense. Fast and frictionless.


## 🔧 Usage Examples

### Basic value checks
```csharp
42.Is(42);       // ✅ passes
42.Is(41);       // ❌ throws IsNotException: 42 (System.Int32) is not 41 (System.Int32)
42.Is(42.0);     // ❌ throws IsNotException: 42 (System.Int32) is not 42 (System.Double)

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
"hello".Is<int>();        // ❌ throws IsNotException: "hello" (System.String) is no System.Int32
```

### Numeric comparisons
```csharp
2.999999f.Is(3f)         // ✅ passes
783.0123.Is(783.0124)    // ✅ passes

5.IsSmallerThan(6);      // ✅ passes
6.IsGreaterThan(5.0);    // ✅ passes
5.IsGreaterThan(6);      // ❌ throws IsNotException: 5 (System.Int32) is not greater than 6 (System.Int32)
2.IsBetween(1, 3);       // ✅ passes

0.3.Is(0.1 + 0.2);                // ✅ passes
0.3.IsExactly(0.1 + 0.2);         // ❌ fails
0.3.IsApproximately(0.1 + 0.2);   // ✅ passes

0.333333.Is(1.0 / 3.0);     // ✅ passes
0.33333.Is(1.0 / 3.0);      // ❌ throws IsNotException: 0,33333 (System.Double) is not close to 0,3333333333333333 (System.Double)
```

### Exception assertions
```csharp
static int DivideByZero(int value) => value / 0;
Action action = () => _ = DivideByZero(1);
action.IsThrowing<DivideByZeroException>();  // ✅ passes

Action action = () => 5.IsGreaterThan(6);
action.IsThrowing<IsNotException>("is not greater than");    // ✅ passes
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

![plot](ErrorMessage.png)


## ⚖️ Design Philosophy

- ❌ No ```.Should()```, no fluent bloat
- ✅ All positive assertions (Is, IsNull, IsTrue, etc.)
- 📢 Failure messages like: ```42 (System.Int32) is not 41 (System.Int32)```
- 🧠 Designed to make tests read like intentions, not machinery


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
