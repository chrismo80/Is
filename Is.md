### Booleans
- **IsTrue**: Asserts that a boolean value is.
- **IsFalse**: Asserts that a boolean value is.
### Collections
- **IsEmpty**: Asserts that the sequence is empty.
- **IsUnique**: Asserts that all elements in the sequence are unique.
- **IsContaining**: Asserts that the `actual` sequence contains all the specified `expected` elements.
- **IsIn**: Asserts that all elements in the `actual` collection are present in the `expected` collection.
- **IsEquivalentTo**: Asserts that the `actual` sequence matches the `expected` sequence ignoring item order.
### Comparisons
- **IsApproximately**: Asserts that the `actual` floating point is approximately equal to `expected` considering an `epsilon`.
- **IsApproximately**: default epsilon is 1e-6.
- **IsPositive**: Asserts that the `actual` numeric value is positive (greater than zero).
- **IsNegative**: Asserts that the `actual` numeric value is negative (less than zero).
- **IsGreaterThan**: Asserts that the `actual` value is greater than the given `other` value.
- **IsSmallerThan**: Asserts that the `actual` value is smaller than the given `other` value.
- **IsAtLeast**: Asserts that the `actual` value is greater or equal the given `other` value.
- **IsAtMost**: Asserts that the `actual` value is smaller or equal the given `other` value.
- **IsBetween**: Asserts that the `actual` value is between `min` and `max` exclusive bounds.
- **IsInRange**: Asserts that the `actual` value is between `min` and `max` inclusive bounds.
- **IsNotBetween**: Asserts that the `actual` value is not between the specified `min` and `max` exclusive bounds.
- **IsOutOfRange**: Asserts that the `actual` value is smaller than `min` or greater than `max`.
- **IsApproximately**: Asserts that the difference between the `actual` and `expected``DateTime` is within the specified `tolerance`.
- **IsApproximately**: Asserts that the difference between the `actual` and `expected``TimeSpan` is within the specified `tolerance`.
### Equality
- **IsExactly**: Asserts that the `actual` object is equal to the `expected` value. (no array unwrapping, exact match for floating points)
- **Is**: Asserts that the `actual` object matches the `expected` value(s). (array unwrapping, approximately for floating points)
- **IsNot**: Asserts that the `actual` value is not equal to the `expected` value.
- **IsSameAs**: Asserts that the `actual` object is the same instance as the `expected` object.
- **IsDefault**: Asserts that the `actual` value is the default value of its type.
- **IsSatisfying**: Asserts that the `actual` object satisfies the specified `predicate`.
- **IsMatchingSnapshot**: Asserts that the given `actual` object matches the `expected` by comparing their serialized JSON strings for equality.
### Exceptions
- **IsThrowing**: Asserts that the given `action` throws an exception of type `T`.
- **IsNotThrowing**: Asserts that the given `action` does not throw an exception of type `T`.
- **IsThrowing**: Asserts that the given synchronous `action` throws an exception of type `T` and that the exception message contains the specified `message` substring.
- **IsThrowing**: Asserts that the given async `function` throws an exception of type `T`.
- **IsNotThrowing**: Asserts that the given async `function` does not throw an exception of type `T`.
- **IsThrowing**: Asserts that the given asynchronous `function` throws an exception of type `T` and that the exception message contains the specified `message` substring.
### Null
- **IsNull**: Asserts that an object is.
- **IsNotNull**: Asserts that the object is not.
### Performance
- **IsCompletingWithin**: Asserts that the given `action` did complete within a specific `timespan`.
- **IsCompletingWithin**: Asserts that the given async `function` did complete within a specific `timespan`.
### Strings
- **IsContaining**: Asserts that the `actual` string contains the specified `expected` substring.
- **IsStartingWith**: Asserts that the `actual` string starts with the specified `expected` string.
- **IsEndingWith**: Asserts that the `actual` string ends with the specified `expected` string.
- **IsMatching**: Asserts that the `actual` string matches the specified `pattern` regular expression.
- **IsNotMatching**: Asserts that the `actual` string does not match the specified `pattern` regular expression.
### Types
- **Is**: Asserts that the actual object is of type `T`.
- **IsNot**: Asserts that the actual object is not of type `T`.
