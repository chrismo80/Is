# Public API
Lines of code < 1000
## Is
#### <u>Configuration</u>
Global configurations that control assertion behaviour.
 
 Can be set via `is.configuration.json`:
 ```
 {
 	"FailureObserver": "Is.FailureObservers.MarkDownObserver, Is",
 	"TestAdapter": "Is.TestAdapters.DefaultAdapter, Is",
 	"AppendCodeLine": true,
 	"ColorizeMessages": true,
 	"FloatingPointComparisonPrecision": 1E-06,
 	"MaxRecursionDepth": 20,
 	"ParsingFlags": 52
 }
 ```
- __`FailureObserver`__: _Determines the observer responsible for handling failure events during assertions. The observer implements logic for capturing, processing, or reporting failures, enabling customisation of diagnostic or reporting mechanisms. Default is `MarkDownObserver`._
- __`TestAdapter`__: _Specifies the adapter responsible for integrating the assertion framework with external testing frameworks. Default is `DefaultAdapter`that is throwing `NotException` for single failures and a `AggregateException` for multiple failures._
- __`AppendCodeLine`__: _Makes code line info in `Failure` optional._
- __`ColorizeMessages`__: _Controls whether messages produced by assertions are colorized when displayed. Default is true, enabling colorization for better readability and visual distinction._
- __`FloatingPointComparisonPrecision`__: _Comparison precision used for floating point comparisons if not specified specifically. Default is 1e-6 (0.000001)._
- __`MaxRecursionDepth`__: _Controls the maximum depth of recursion when parsing deeply nested objects. Default is 20._
- __`ParsingFlags`__: _Controls the binding flags to use when parsing deeply nested objects. Default is public | non-public | instance._
- __`JsonSerializerOptions`__: _These options dictate aspects such as how JSON properties are written, ignored, or formatted, enabling fine-grained control over the serialization processes._
## Is.Assertions
All assertions are implemented as extension methods.
#### <u>Booleans</u>
- __`IsTrue(actual)`__: _Asserts that a boolean value is `true`._
- __`IsFalse(actual)`__: _Asserts that a boolean value is `false`._
#### <u>Collections</u>
- __`IsEmpty<T>(actual)`__: _Asserts that the sequence is empty._
- __`IsNotEmpty<T>(actual)`__: _Asserts that the sequence is not empty._
- __`IsUnique<T>(actual)`__: _Asserts that all elements in the sequence are unique._
- __`IsContaining<T>(actual, expected)`__: _Asserts that the `actual` sequence contains all the specified `expected` elements._
- __`IsContaining<T>(actual, expected)`__: _Asserts that the sequence contains the specified elements._
- __`IsNotContaining<T>(actual, expected)`__: _Asserts that the sequence does not contain the specified elements._
- __`IsIn<T>(actual, expected)`__: _Asserts that all elements in the `actual` collection are present in the `expected` collection._
- __`IsIn<T>(actual, expected)`__: _Checks that the specified element is contained within the given sequence._
- __`IsNotIn<T>(actual, expected)`__: _Checks that the specified element is not contained within the given sequence._
- __`IsOrdered<T>(actual)`__: _Asserts that the sequence is ordered in ascending order._
- __`IsOrderedDescending<T>(actual)`__: _Asserts that the sequence is ordered in descending order._
- __`IsEquivalentTo<T>(actual, expected)`__: _Asserts that the `actual` sequence matches the `expected` sequence ignoring item order by using Default Equality comparer of `T`._
- __`IsDeeplyEquivalentTo<T>(actual, expected, ignorePaths)`__: _Asserts that the `actual` sequence matches the `expected` sequence ignoring item order by using Deeply Equality comparer of `T`._
- __`IsEquivalentTo<TKey, T>(actual, expected, ignoreKeys)`__: _Asserts that the `actual` dictionary matches the `expected` dictionary ignoring order. Optional predicate can be used to ignore specific keys._
#### <u>Comparisons</u>
- __`IsApproximately<T>(actual, expected, precision)`__: _Asserts that the `actual` floating point is approximately equal to `expected` considering an `precision`._
- __`IsApproximately<T>(actual, expected)`__: _Uses default precision from configuration_
- __`IsPositive<T>(actual)`__: _Asserts that the `actual` numeric value is positive (greater than zero)._
- __`IsNegative<T>(actual)`__: _Asserts that the `actual` numeric value is negative (less than zero)._
- __`IsGreaterThan<T>(actual, other)`__: _Asserts that the `actual` value is greater than the given `other` value._
- __`IsSmallerThan<T>(actual, other)`__: _Asserts that the `actual` value is smaller than the given `other` value._
- __`IsAtLeast<T>(actual, other)`__: _Asserts that the `actual` value is greater or equal the given `other` value._
- __`IsAtMost<T>(actual, other)`__: _Asserts that the `actual` value is smaller or equal the given `other` value._
- __`IsBetween<T>(actual, min, max)`__: _Asserts that the `actual` value is between `min` and `max` exclusive bounds._
- __`IsInRange<T>(actual, min, max)`__: _Asserts that the `actual` value is between `min` and `max` inclusive bounds._
- __`IsNotBetween<T>(actual, min, max)`__: _Asserts that the `actual` value is not between the specified `min` and `max` exclusive bounds._
- __`IsOutOfRange<T>(actual, min, max)`__: _Asserts that the `actual` value is smaller than `min` or greater than `max`._
#### <u>DateTimes</u>
- __`IsApproximately(actual, expected, tolerance)`__: _Asserts that the difference between two `DateTime` is within the specified `tolerance`._
- __`IsApproximately(actual, expected, tolerance)`__: _Asserts that the difference between two `TimeSpan` is within the specified `tolerance`._
- __`IsSameDay(actual, other)`__: _Determines whether the specified `actual` date and `other` date are on the same calendar day._
- __`IsOlderThan(actual, years, date)`__: _Checks that the given `actual` date is older than the specified number of `years` relative to the reference date `date` or today if not provided._
- __`IsOlderThan(actual, years)`__: _Checks that the given `actual` date is older than the specified number of `years`._
#### <u>Delegates</u>
- __`IsThrowing<T>(action)`__: _Asserts that the given `action` throws an exception of type `T`._
- __`IsNotThrowing<T>(action)`__: _Asserts that the given `action` does not throw an exception of type `T`._
- __`IsThrowing<T>(action, message)`__: _Asserts that the given synchronous `action` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __`IsThrowing<T>(function)`__: _Asserts that the given async `function` throws an exception of type `T`._
- __`IsNotThrowing<T>(function)`__: _Asserts that the given async `function` does not throw an exception of type `T`._
- __`IsThrowing<T>(function, message)`__: _Asserts that the given asynchronous `function` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __`IsCompletingWithin(action, timespan)`__: _Asserts that the given `action` did complete within a specific `timespan`._
- __`IsCompletingWithin(function, timespan)`__: _Asserts that the given async `function` did complete within a specific `timespan`._
- __`IsAllocatingAtMost(action, kiloBytes)`__: _Asserts that the given `action` is allocating not more than `kiloBytes`._
- __`IsAllocatingAtMost(function, kiloBytes)`__: _Asserts that the given async `function` is allocating not more than `kiloBytes`._
#### <u>Equality</u>
- __`IsExactly<T>(actual, expected)`__: _Asserts that the `actual` object is equal to the `expected` value. (no array unwrapping, exact match for floating points)_
- __`Is(actual, expected)`__: _Asserts that the `actual` object matches the `expected` value(s). (array unwrapping, approximately for floating points)_
- __`IsNot<T>(actual, expected)`__: _Asserts that the `actual` value is not equal to the `expected` value._
- __`IsSameAs<T>(actual, expected)`__: _Asserts that the `actual` object is the same instance as the `expected` object._
- __`IsDefault<T>(actual)`__: _Asserts that the `actual` value is the default value of its type._
- __`IsSatisfying<T>(actual, predicate)`__: _Asserts that the `actual` object satisfies the specified `predicate`._
- __`IsMatchingSnapshot(actual, expected, ignorePaths, options)`__: _Asserts that the given `actual` object matches the `expected` by comparing their serialized JSON strings for equality. Optional predicate can be used to ignore specific paths._
- __`IsMatching(actual, other, ignorePaths)`__: _Asserts that the given `actual` object matches the `other` by running a deep reflection-based object comparison on their properties and fields for equality. Optional predicate can be used to ignore specific paths._
#### <u>Null</u>
- __`IsNull(actual)`__: _Asserts that an object is `null`._
- __`IsNotNull(actual)`__: _Asserts that the object is not `null`._
#### <u>Strings</u>
- __`IsBlank(actual)`__: _Asserts that the `actual` string is `null` or empty or contains only whitespaces._
- __`IsNotBlank(actual)`__: _Asserts that the `actual` string is not `null` and not empty and does not contain only whitespaces_
- __`IsContaining(actual, expected)`__: _Asserts that the `actual` string contains the specified `expected` substring._
- __`IsStartingWith(actual, expected)`__: _Asserts that the `actual` string starts with the specified `expected` string._
- __`IsEndingWith(actual, expected)`__: _Asserts that the `actual` string ends with the specified `expected` string._
- __`IsEquivalentTo(actual, expected)`__: _Asserts that the `actual` string is equivalent to the `expected` string, ignoring case differences._
- __`IsMatching(actual, pattern)`__: _Asserts that the `actual` string matches the specified `pattern` regular expression._
- __`IsNotMatching(actual, pattern)`__: _Asserts that the `actual` string does not match the specified `pattern` regular expression._
- __`IsEmail(actual)`__: _Determines whether the `actual` string is a valid email address._
#### <u>Types</u>
- __`Is<T>(actual)`__: _Asserts that the actual object is of type `T`._
- __`IsNot<T>(actual)`__: _Asserts that the actual object is not of type `T`._
## Is.Core
#### <u>AssertionContext</u>
Represents a scoped context that captures all assertion failures within its lifetime and reports the collection upon disposal if any failures occurred.
 Intended for test frameworks like NUnit, xUnit, or MSTest. By using this context, tests can collect multiple
 assertion failures and evaluate them together at the end of the test unless being dequeued before.
 
 Usage:
 ```
 using var context = AssertionContext.Begin();
 
 false.IsTrue();
 4.Is(5);
 
 context.Failed.Is(2);
 context.NextFailure().Message.IsContaining("false.IsTrue()");
 
 context.Failed.Is(1);
 context.NextFailure().Message.IsContaining("4.Is(5)");
 ```
 
 If any failures are not dequeued or handled manually, they will be reported when the context is disposed.
 
- __`Current`__: _The current active `AssertionContext` for the asynchronous operation, or null if no context is active._
- __`Begin(method)`__: _Starts a new `AssertionContext` on the current thread. Current `Configuration` is cloned to enable local configurations per assertion context._
- __`Dispose()`__: _Ends the assertion context and reports all collected failures to the `!:ITestAdapter`_
- __`NextFailure()`__: _Dequeues an `Failure` from the queue to not be reported at the end of the context._
- __`TakeFailures(count)`__: _Dequeues as many `Failure`s specified in `count` from the queue._
#### <u>Check</u>
Offers a fluent API to assert conditions and create return values and error messages. Can be used for custom assertions
- __`That(condition)`__: _Evaluates a boolean condition._
- __`Arg(condition, message)`__: _Guard clause for argument checks, throws `ArgumentException`_
- __`Op(condition, message)`__: _Guard clause for operation checks, throws `InvalidOperationException`_
- __`Yields<TResult>(result)`__: _Projects a result from the original value if the initial predicate condition was true._
- __`Unless(actual, message, other)`__: _Returns the result if the condition is true; otherwise, triggers a failure with a message._
#### <u>Failure</u>
Represents a failure encountered during an assertion or test execution. Contains detailed information about the failure, including message, actual and expected values, assertion details, and location in source code.
- __`Created`__: _The date and time when the failure instance was created._
- __`Message`__: _The failure message._
- __`Actual`__: _The actual value that caused the assertion to fail._
- __`Expected`__: _The expected value that was compared during the assertion and caused the failure._
- __`Assertion`__: _The name of the assertion that failed._
- __`Method`__: _The name of the method that called the assertion, or null if unavailable._
- __`File`__: _The name of the file in which the exception occurred, if available._
- __`Line`__: _The line number in the source file where the exception occurred._
- __`Code`__: _The specific line of source code of the assertion failure._
#### <u>IsAssertionAttribute</u>
Mark custom assertion methods with this attribute to enable proper code line detection.
 Usage:
 ```
 [IsAssertion]
 public static bool IsCustomAssertion(this int value, [CallerArgumentExpression("value")] string? expr = null) =>
 	Check.That(value > 0).Unless(value, $"in '{expr}' is not positive");
 ```
#### <u>IsAssertionsAttribute</u>
Mark a custom assertions class with this attribute to enable proper code line detection.
## Is.Core.Interfaces
#### <u>IFailureObserver</u>
Interface providing a mechanism to observe failures. Can be set via Configuration.FailureObserver.
- __`OnFailure(failure)`__: _This method is invoked when a failure occurs during an assertion. Observer can perform custom logic on that failure such as logging or reporting._
#### <u>ITestAdapter</u>
Represents an interface for handling test result reporting. Serves as a hook for custom test frameworks to throw custom exception types. Can be set via Configuration.TestAdapter.
- __`ReportFailure(failure)`__: _Reports a failed test result to the configured test adapter._
- __`ReportFailures(message, failures)`__: _Reports multiple test failures to the configured test adapter._
## Is.FailureObservers
#### <u>ConsoleObserver</u>
`IFailureObserver` that writes all failures directly to the Console.
#### <u>JsonObserver</u>
`IFailureObserver` that writes all failures into one FailureReport JSON file.
#### <u>MarkDownObserver</u>
`IFailureObserver` that writes all failures into one FailureReport mark-down file.
## Is.TestAdapters
#### <u>CustomExceptionAdapter</u>
`ITestAdapter` that allows throwing a custom exception type `TException` when test failures are reported.
#### <u>DefaultAdapter</u>
`ITestAdapter` that is throwing `NotException`s for single failures and an `AggregateException` for multiple failures.
#### <u>UnitTestAdapter</u>
`ITestAdapter` that is throwing test framework specific exception. Detects the loaded framework (MS Test, xUnit, NUnit) on first failure. If none of those is detected `NotException`(s) are thrown.
## Is.Tools
#### <u>JsonFileHelper</u>
- __`SaveJson<T>(obj, filename)`__: _Serializes an object `obj` to a JSON file to `filename`_
- __`LoadJson<T>(filename)`__: _Deserializes an object to type `T` from a JSON file at `filename`_
