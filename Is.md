# Public API
All public methods are designed as extensions methods.
## Classes
- __Core__: _Represents a scoped context that captures all assertion failures (as `NotException` instances) within its lifetime and throws a single `AggregateException` upon disposal if any failures occurred._
- __Core__: _This exception is thrown when an assertion fails and `ThrowOnFailure` is enabled. When used inside an `AssertionContext`, instances of `NotException` are collected instead of being thrown immediately._
## Properties
#### <u>Core</u>
- __AssertionContext.Failed__: _Gets the number of remaining assertion failures in the context._
- __Configuration.ThrowOnFailure__: _Gets or sets a value indicating whether assertion failures should throw a `NotException`. Default is true. If set to false, assertions will return false on failure and log the message._
- __Configuration.Logger__: _Gets or sets the logger delegate to use when `ThrowOnFailure` is false. Default case, messages will be written to `Debug.WriteLine`._
- __Configuration.FloatingPointComparisonFactor__: _Default value used for floating point comparisons if not specified specifically_
- __Configuration.AppendCodeLine__: _Makes code line info in `NotException` optional_
## Methods
#### <u>Assertions</u>
- __Booleans.IsTrue__: _Asserts that a boolean value is `true`._
- __Booleans.IsFalse__: _Asserts that a boolean value is `false`._
- __Collections.IsEmpty__: _Asserts that the sequence is empty._
- __Collections.IsUnique__: _Asserts that all elements in the sequence are unique._
- __Collections.IsContaining__: _Asserts that the `actual` sequence contains all the specified `expected` elements._
- __Collections.IsIn__: _Asserts that all elements in the `actual` collection are present in the `expected` collection._
- __Collections.IsEquivalentTo__: _Asserts that the `actual` sequence matches the `expected` sequence ignoring item order._
- __Collections.IsEquivalentTo__: _Asserts that the `actual` dictionary matches the `expected` dictionary ignoring order. Optional predicate can be used to ignore specific keys._
- __Comparisons.IsApproximately__: _Asserts that the `actual` floating point is approximately equal to `expected` considering an `factor`._
- __Comparisons.IsApproximately__: _uses default value from configuration_
- __Comparisons.IsPositive__: _Asserts that the `actual` numeric value is positive (greater than zero)._
- __Comparisons.IsNegative__: _Asserts that the `actual` numeric value is negative (less than zero)._
- __Comparisons.IsGreaterThan__: _Asserts that the `actual` value is greater than the given `other` value._
- __Comparisons.IsSmallerThan__: _Asserts that the `actual` value is smaller than the given `other` value._
- __Comparisons.IsAtLeast__: _Asserts that the `actual` value is greater or equal the given `other` value._
- __Comparisons.IsAtMost__: _Asserts that the `actual` value is smaller or equal the given `other` value._
- __Comparisons.IsBetween__: _Asserts that the `actual` value is between `min` and `max` exclusive bounds._
- __Comparisons.IsInRange__: _Asserts that the `actual` value is between `min` and `max` inclusive bounds._
- __Comparisons.IsNotBetween__: _Asserts that the `actual` value is not between the specified `min` and `max` exclusive bounds._
- __Comparisons.IsOutOfRange__: _Asserts that the `actual` value is smaller than `min` or greater than `max`._
- __Comparisons.IsApproximately__: _Asserts that the difference between two `DateTime` is within the specified `tolerance`._
- __Comparisons.IsApproximately__: _Asserts that the difference between two `TimeSpan` is within the specified `tolerance`._
- __Delegates.IsThrowing__: _Asserts that the given `action` throws an exception of type `T`._
- __Delegates.IsNotThrowing__: _Asserts that the given `action` does not throw an exception of type `T`._
- __Delegates.IsThrowing__: _Asserts that the given synchronous `action` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __Delegates.IsThrowing__: _Asserts that the given async `function` throws an exception of type `T`._
- __Delegates.IsNotThrowing__: _Asserts that the given async `function` does not throw an exception of type `T`._
- __Delegates.IsThrowing__: _Asserts that the given asynchronous `function` throws an exception of type `T` and that the exception message contains the specified `message` substring._
- __Delegates.IsCompletingWithin__: _Asserts that the given `action` did complete within a specific `timespan`._
- __Delegates.IsCompletingWithin__: _Asserts that the given async `function` did complete within a specific `timespan`._
- __Delegates.IsAllocatingAtMost__: _Asserts that the given `action` is allocating not more than `kiloBytes`._
- __Delegates.IsAllocatingAtMost__: _Asserts that the given async `function` is allocating not more than `kiloBytes`._
- __Equality.IsExactly__: _Asserts that the `actual` object is equal to the `expected` value. (no array unwrapping, exact match for floating points)_
- __Equality.Is__: _Asserts that the `actual` object matches the `expected` value(s). (array unwrapping, approximately for floating points)_
- __Equality.IsNot__: _Asserts that the `actual` value is not equal to the `expected` value._
- __Equality.IsSameAs__: _Asserts that the `actual` object is the same instance as the `expected` object._
- __Equality.IsDefault__: _Asserts that the `actual` value is the default value of its type._
- __Equality.IsSatisfying__: _Asserts that the `actual` object satisfies the specified `predicate`._
- __Equality.IsMatchingSnapshot__: _Asserts that the given `actual` object matches the `expected` by comparing their serialized JSON strings for equality. Optional predicate can be used to ignore specific paths._
- __Equality.IsMatching__: _Asserts that the given `actual` object matches the `other` by running a deep reflection-based object comparison on their properties and fields for equality. Optional predicate can be used to ignore specific paths._
- __Null.IsNull__: _Asserts that an object is `null`._
- __Null.IsNotNull__: _Asserts that the object is not `null`._
- __Strings.IsContaining__: _Asserts that the `actual` string contains the specified `expected` substring._
- __Strings.IsStartingWith__: _Asserts that the `actual` string starts with the specified `expected` string._
- __Strings.IsEndingWith__: _Asserts that the `actual` string ends with the specified `expected` string._
- __Strings.IsMatching__: _Asserts that the `actual` string matches the specified `pattern` regular expression._
- __Strings.IsNotMatching__: _Asserts that the `actual` string does not match the specified `pattern` regular expression._
- __Types.Is__: _Asserts that the actual object is of type `T`._
- __Types.IsNot__: _Asserts that the actual object is not of type `T`._
#### <u>Core</u>
- __AssertionContext.Begin__: _Starts a new `AssertionContext` on the current thread. All assertion failures will be collected and thrown as an `AggregateException` when the context is disposed._
- __AssertionContext.Dispose__: _Ends the assertion context and validates all collected failures. If any assertions failed, throws an `AggregateException` containing all collected `NotException`s._
- __AssertionContext.NextFailure__: _Dequeues an `NotException` from the queue to not be thrown at the end of the context._
- __NotException.#ctor__: _This exception is thrown when an assertion fails and `ThrowOnFailure` is enabled. When used inside an `AssertionContext`, instances of `NotException` are collected instead of being thrown immediately._
#### <u>Tools</u>
- __JsonFileHelper.SaveJson__: _Serializes an object `obj` to a JSON file to `filename`_
- __JsonFileHelper.LoadJson__: _Deserializes an object to type `T` from a JSON file at `filename`_
## Classes
#### <u>Core</u>
- __AssertionContext__: _Represents a scoped context that captures all assertion failures (as `NotException` instances) within its lifetime and throws a single `AggregateException` upon disposal if any failures occurred._
- __NotException__: _This exception is thrown when an assertion fails and `ThrowOnFailure` is enabled. When used inside an `AssertionContext`, instances of `NotException` are collected instead of being thrown immediately._
