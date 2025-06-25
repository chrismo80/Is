# Public API
Lines of code: 610
## Is
#### <u>NotException</u>
This exception is thrown when an assertion fails and `ThrowOnFailure` is enabled. When used inside an `AssertionContext`, instances of `NotException` are collected instead of being thrown immediately.
#### <u>Configuration</u>
- __ThrowOnFailure__: _Controls whether assertion failures should throw a `NotException`. Default is true. If not set, assertions will return false on failure and log the message._
- __Logger__: _A logger delegate to use when `ThrowOnFailure` is false. Default case, messages will be written to `Debug.WriteLine`._
- __AppendCodeLine__: _Makes code line info in `NotException` optional._
- __FloatingPointComparisonPrecision__: _Comparison precision used for floating point comparisons if not specified specifically. Default is 1e-6 (0.000001)._
- __MaxRecursionDepth__: _Controls the maximum depth of recursion when parsing deeply nested objects. Default is 20._
- __ParsingFlags__: _Controls the binding flags to use when parsing deeply nested objects. Default is public | non-public | instance._
## Is.Assertions
#### <u>Booleans</u>
- __IsTrue()__: _Asserts that a boolean value is `true`._
- __IsFalse()__: _Asserts that a boolean value is `false`._
#### <u>Collections</u>
- __IsEmpty()__: _Asserts that the sequence is empty._
- __IsUnique()__: _Asserts that all elements in the sequence are unique._
- __IsContaining()__: _Asserts that the `actual` sequence contains all the specified `expected` elements._
- __IsIn()__: _Asserts that all elements in the `actual` collection are present in the `expected` collection._
- __IsEquivalentTo()__: _Asserts that the `actual` sequence matches the `expected` sequence ignoring item order._
#### <u>Comparisons</u>
- __IsApproximately()__: _Asserts that the `actual` floating point is approximately equal to `expected` considering an `factor`._
- __IsPositive()__: _Asserts that the `actual` numeric value is positive (greater than zero)._
- __IsNegative()__: _Asserts that the `actual` numeric value is negative (less than zero)._
- __IsGreaterThan()__: _Asserts that the `actual` value is greater than the given `other` value._
- __IsSmallerThan()__: _Asserts that the `actual` value is smaller than the given `other` value._
- __IsAtLeast()__: _Asserts that the `actual` value is greater or equal the given `other` value._
- __IsAtMost()__: _Asserts that the `actual` value is smaller or equal the given `other` value._
- __IsBetween()__: _Asserts that the `actual` value is between `min` and `max` exclusive bounds._
- __IsInRange()__: _Asserts that the `actual` value is between `min` and `max` inclusive bounds._
- __IsNotBetween()__: _Asserts that the `actual` value is not between the specified `min` and `max` exclusive bounds._
- __IsOutOfRange()__: _Asserts that the `actual` value is smaller than `min` or greater than `max`._
#### <u>Delegates</u>
- __IsThrowing()__: _Asserts that the given `action` throws an exception of type `T`._
- __IsNotThrowing()__: _Asserts that the given `action` does not throw an exception of type `T`._
- __IsCompletingWithin()__: _Asserts that the given `action` did complete within a specific `timespan`._
- __IsAllocatingAtMost()__: _Asserts that the given `action` is allocating not more than `kiloBytes`._
#### <u>Equality</u>
- __IsExactly()__: _Asserts that the `actual` object is equal to the `expected` value. (no array unwrapping, exact match for floating points)_
- __IsNot()__: _Asserts that the `actual` value is not equal to the `expected` value._
- __IsSameAs()__: _Asserts that the `actual` object is the same instance as the `expected` object._
- __IsDefault()__: _Asserts that the `actual` value is the default value of its type._
- __IsSatisfying()__: _Asserts that the `actual` object satisfies the specified `predicate`._
- __IsMatchingSnapshot()__: _Asserts that the given `actual` object matches the `expected` by comparing their serialized JSON strings for equality. Optional predicate can be used to ignore specific paths._
#### <u>Null</u>
- __IsNull()__: _Asserts that an object is `null`._
- __IsNotNull()__: _Asserts that the object is not `null`._
#### <u>Strings</u>
- __IsContaining()__: _Asserts that the `actual` string contains the specified `expected` substring._
- __IsStartingWith()__: _Asserts that the `actual` string starts with the specified `expected` string._
- __IsEndingWith()__: _Asserts that the `actual` string ends with the specified `expected` string._
- __IsMatching()__: _Asserts that the `actual` string matches the specified `pattern` regular expression._
- __IsNotMatching()__: _Asserts that the `actual` string does not match the specified `pattern` regular expression._
#### <u>Types</u>
- __Is()__: _Asserts that the actual object is of type `T`._
- __IsNot()__: _Asserts that the actual object is not of type `T`._
## Is.Core
#### <u>AssertionContext</u>
Represents a scoped context that captures all assertion failures (as `NotException` instances) within its lifetime and throws a single `AggregateException` upon disposal if any failures occurred.
- __Failed__: _Gets the number of remaining assertion failures in the context._
- __Passed__: _Gets the number of passed assertions in the context._
- __Total__: _Gets the total number of assertions in the context._
- __Ratio__: _Gets the ratio of passed assertions._
- __Begin()__: _Starts a new `AssertionContext` on the current thread. All assertion failures will be collected and thrown as an `AggregateException` when the context is disposed._
- __Dispose()__: _Ends the assertion context and validates all collected failures. If any assertions failed, throws an `AggregateException` containing all collected `NotException`s._
- __NextFailure()__: _Dequeues an `NotException` from the queue to not be thrown at the end of the context._
#### <u>Check</u>
Offers a fluent API to assert conditions and create return values and error messages. Can be used for custom assertions
- __That()__: _Evaluates a boolean condition._
- __Yields()__: _Projects a result from the original value if the initial predicate condition was true._
- __Unless()__: _Returns the result if the condition is true; otherwise, triggers a failure with a message._
#### <u>IsExtensionAttribute</u>
Mark custom assertions with this attribute to enable proper code line detection.
## Is.Tools
#### <u>JsonFileHelper</u>
- __SaveJson()__: _Serializes an object `obj` to a JSON file to `filename`_
- __LoadJson()__: _Deserializes an object to type `T` from a JSON file at `filename`_
