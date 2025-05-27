# 📦 Is – Minimalistic Assertion Extensions for .NET

**Simple. Readable. Opinionated.**

`Is` is a lightweight assertion library for .NET that focuses on _readable_, _minimal_, and _fail-fast_ test expectations — no assertion clutter, no dependencies, no test framework lock-in.


## ✅ Why use Is?

- 📘 Concise: One word. One assertion.
- 💥 Opinionated: No "IsNot", no message matching, just fast failure and clarity.
- 🧪 Test-framework agnostic: Works with xUnit, NUnit, MSTest, or none at all.
- ⚙️ Self-contained: No dependencies, no configuration, just drop it in.




## 🔍 Available Methods

All public methods in `Is` are:

- ❗️ **Extension methods**, designed to be used fluently (`value.Is(...)`)
- 🔤 **Named consistently**: Every method starts with `Is`, making them easy to discover with IntelliSense
- ✂️ **Minimal and deliberate**: Only a small, opinionated set of assertions is exposed

| Method                             | Description                                                                 |
| ---------------------------------- | --------------------------------------------------------------------------- |
| 🔥 `IsThrowing<T>()`               | Asserts that an `Action` throws an exception of type `T`                    |
| 🔥 `IsThrowing<T>(string)`         | Asserts that an `Action` throws and the exception message contains a string |
| 🔥 `IsThrowing<T>()` (async)       | Asserts that an async `Func<Task>` throws an exception of type `T`          |
| 🔥 `IsThrowing<T>(string)` (async) | Asserts that an async `Func<Task>` throws and the message contains a string |
| 🔠 `Is<T>()`                       | Asserts that the value is of type `T`                                       |
| 🔠 `Is(params object[])`           | Asserts that the value matches any expected value                           |
| 🔠 `IsExactly(object)`             | Asserts strict value equality                                               |
| 📏 `IsGreaterThan(T)`              | Asserts that the value is greater than another                              |
| 📏 `IsSmallerThan(T)`              | Asserts that the value is smaller than another                              |
| 📏 `IsBetween(min, max)`           | Asserts that the value is between two bounds                                |
| 📦 `IsEmpty()`                     | Asserts that an `IEnumerable` is empty                                      |
| 📦 `IsContaining(params T[])`      | Asserts that an `IEnumerable` contains all specified elements               |
| 📦 `IsContaining(string)`          | Asserts that a `string` contains a substring                                |
| 🚫 `IsNull()`                      | Asserts that a value is `null`                                              |
| ✅ `IsTrue()`                       | Asserts that a `bool` is `true`                                             |
| ❌ `IsFalse()`                      | Asserts that a `bool` is `false`                                            |


> ✅ Because all methods start with `Is`, you can type `.` and just filter by `Is` in IntelliSense. Fast and frictionless.


## 🔧 Usage Examples

### Basic value checks
```csharp
42.Is(42);       // ✅ passes
42.Is(41);       // ❌ throws IsNotException: 42 (System.Int32) is not 41 (System.Int32)
42.Is(42.0);     // ❌ throws IsNotException: 42 (System.Int32) is not 42 (System.Double)

"test".Is("test");               // ✅ passes

new[] { 1, 2, 3 }.Is(1, 2, 3);   // ✅ passes (enumerable values check)

new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4, 6);     // ✅ passes
new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 3 == 0).Is(3, 6);        // ✅ passes
new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 4 == 0).Is(4);           // ✅ passes
```

### Type checks
```csharp
"hello".Is<string>();       // ✅ passes
"hello".Is<int>();          // ❌ throws IsNotException: "hello" (System.String) is no System.Int32
```

### Numeric comparisons
```csharp
2.999999f.Is(3f)            // ✅ passes
783.0123.Is(783.0124)       // ✅ passes

5.IsSmallerThan(6);         // ✅ passes
6.IsGreaterThan(5.0);       // ✅ passes
5.IsGreaterThan(6);         // ❌ throws IsNotException: 5 (System.Int32) is not greater than 6 (System.Int32)

0.3.Is(0.1 + 0.2);          // ✅ passes
0.333333.Is(1.0 / 3.0);     // ✅ passes
0.33333.Is(1.0 / 3.0);      // ❌ throws IsNotException: 0,33333 (System.Double) is not close to 0,3333333333333333 (System.Double)
```

### Exception assertions
```csharp
static int DivideByZero(int value) => value / 0;
Action action = () => _ = DivideByZero(1);
action.IsThrowing<DivideByZeroException>();  // ✅ passes

Action action = () => 5.IsGreaterThan(6);
action.IsThrowing<IsNotException>().Message.Contains("is not greater than").IsTrue();    // ✅ passes
```


## ⚖️ Design Philosophy

- ❌ No ```.Should()```, no fluent bloat
- ✅ All positive assertions (Is, IsNull, IsTrue, etc.)
- 📢 Failure messages like: ```42 (System.Int32) is not 41 (System.Int32)```
- 🧠 Designed to make tests read like intentions, not machinery


## 📝 License

MIT – use freely.

## 🙌 Contributing

Ideas, bug reports, or pull requests are always welcome.

## ❤️ Author

Developed with care by chrismo80
