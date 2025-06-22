# Public API
All public methods are designed as extensions methods.

Lines of code: 626
## Classes
- __AssertionContext__: _Represents a scoped context that captures all assertion failures (as `NotException` instances) within its lifetime and throws a single `AggregateException` upon disposal if any failures occurred._
- __IsExtensionAttribute__: _Mark custom assertions with this attribute to enable proper code line detection._
- __NotException__: _This exception is thrown when an assertion fails and `ThrowOnFailure` is enabled. When used inside an `AssertionContext`, instances of `NotException` are collected instead of being thrown immediately._
## Properties
#### <u>AssertionContext</u>
- __Failed__: _Gets the number of remaining assertion failures in the context._
- __Passed__: _Gets the number of passed assertions in the context._
- __Total__: _Gets the total number of assertions in the context._
- __Ratio__: _Gets the ratio of passed assertions._
#### <u>Configuration</u>
- __ThrowOnFailure__: _Gets or sets a value indicating whether assertion failures should throw a `NotException`. Default is true. If set to false, assertions will return false on failure and log the message._
- __Logger__: _Gets or sets the logger delegate to use when `ThrowOnFailure` is false. Default case, messages will be written to `Debug.WriteLine`._
- __FloatingPointComparisonFactor__: _Default value used for floating point comparisons if not specified specifically_
- __AppendCodeLine__: _Makes code line info in `NotException` optional_
- __MaxRecursionDepth__: _Controls the maximum depth of recursion when parsing deeply nested objects_
- __ParsingFlags__: _Controls the binding flags to use when parsing deeply nested objects_
## Methods
#### <u>Booleans</u>
- __IsTrue__: _Asserts that a boolean value is `true`._
- __IsFalse__: _Asserts that a boolean value is `false`._
#### <u>Collections</u>
- __IsEmpty__: _Asserts that the sequence is empty._
- __IsUnique__: _Asserts that all elements in the sequence are unique._
- __IsContaining__: _Asserts that the `actual` sequence contains all the specified `expected` elements._
- __IsIn__: _Asserts that all elements in the `actual` collection are present in the `expected` collection._
- __IsEquivalentTo__: _Asserts that the `actual` sequence matches the `expected` sequence ignoring item order._
- __IsEquivalentTo__: _Asserts that the `actual` dictionary matches the `expected` dictionary ignoring order. Optional predicate can be used to ignore specific keys._
#### <u>Comparisons</u>
- __IsApproximately__: _Asserts that the `actual` floating point is approximately equal to `expected` considering an `factor`._
- __IsApproximately__: _uses default value from configuration_
- __IsPositive__: _Asserts that the `actual` numeric value is positive (greater than zero)._
- __IsNegative__: _Asserts that the `actual` numeric value is negative (less than zero)._
- __IsGreaterThan__: _Asserts that the `actual` value is greater than the given `other` value._
- __IsSmallerThan__: _Asserts that the `actual` value is smaller than the given `other` value._
- __IsAtLeast__: _Asserts that the `actual` value is greater or equal the given `other` value._
- __IsAtMost__: _Asserts that the `actual` value is smaller or equal the given `other` value._
- __IsBetween__: _Asserts that the `actual` value is between `min` and `max` exclusive bounds._
- __IsInRange__: _Asserts that the `actual` value is between `min` and `max` inclusive bounds._
- __IsNotBetween__: _Asserts that the `actual` value is not between the specified `min` and `max` exclusive bounds._
- __IsOutOfRange__: _Asserts that the `actual` value is smaller than `min` or greater than `max`._
- __IsApproximately__: _Asserts that the difference between two `DateTime` is within the specified `tolerance`._
- __IsApproximately__: _Asserts that the difference between two `TimeSpan` is within the specified `tolerance`._
#### <u>Delegates</u>
- __IsThrowing__: _Asserts that the given `action` throws an exception of type `T`._
- __IsNotThrowing__: _Asserts that the given `action` does not throw an exception of type `T`._
- __IsThrowing__: _Asserts that the given synchronous `action` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __IsThrowing__: _Asserts that the given async `function` throws an exception of type `T`._
- __IsNotThrowing__: _Asserts that the given async `function` does not throw an exception of type `T`._
- __IsThrowing__: _Asserts that the given asynchronous `function` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __IsCompletingWithin__: _Asserts that the given `action` did complete within a specific `timespan`._
- __IsCompletingWithin__: _Asserts that the given async `function` did complete within a specific `timespan`._
- __IsAllocatingAtMost__: _Asserts that the given `action` is allocating not more than `kiloBytes`._
- __IsAllocatingAtMost__: _Asserts that the given async `function` is allocating not more than `kiloBytes`._
#### <u>Equality</u>
- __IsExactly__: _Asserts that the `actual` object is equal to the `expected` value. (no array unwrapping, exact match for floating points)_
- __Is__: _Asserts that the `actual` object matches the `expected` value(s). (array unwrapping, approximately for floating points)_
- __IsNot__: _Asserts that the `actual` value is not equal to the `expected` value._
- __IsSameAs__: _Asserts that the `actual` object is the same instance as the `expected` object._
- __IsDefault__: _Asserts that the `actual` value is the default value of its type._
- __IsSatisfying__: _Asserts that the `actual` object satisfies the specified `predicate`._
- __IsMatchingSnapshot__: _Asserts that the given `actual` object matches the `expected` by comparing their serialized JSON strings for equality. Optional predicate can be used to ignore specific paths._
- __IsMatching__: _Asserts that the given `actual` object matches the `other` by running a deep reflection-based object comparison on their properties and fields for equality. Optional predicate can be used to ignore specific paths._
#### <u>Null</u>
- __IsNull__: _Asserts that an object is `null`._
- __IsNotNull__: _Asserts that the object is not `null`._
#### <u>Strings</u>
- __IsContaining__: _Asserts that the `actual` string contains the specified `expected` substring._
- __IsStartingWith__: _Asserts that the `actual` string starts with the specified `expected` string._
- __IsEndingWith__: _Asserts that the `actual` string ends with the specified `expected` string._
- __IsMatching__: _Asserts that the `actual` string matches the specified `pattern` regular expression._
- __IsNotMatching__: _Asserts that the `actual` string does not match the specified `pattern` regular expression._
#### <u>Types</u>
- __Is__: _Asserts that the actual object is of type `T`._
- __IsNot__: _Asserts that the actual object is not of type `T`._
#### <u>AssertionContext</u>
- __Begin__: _Starts a new `AssertionContext` on the current thread. All assertion failures will be collected and thrown as an `AggregateException` when the context is disposed._
- __Dispose__: _Ends the assertion context and validates all collected failures. If any assertions failed, throws an `AggregateException` containing all collected `NotException`s._
- __NextFailure__: _Dequeues an `NotException` from the queue to not be thrown at the end of the context._
#### <u>JsonFileHelper</u>
- __SaveJson__: _Serializes an object `obj` to a JSON file to `filename`_
- __LoadJson__: _Deserializes an object to type `T` from a JSON file at `filename`_
