namespace Is.Core;

/// <summary>
/// Mark custom assertion methods with this attribute to enable proper code line detection.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// [IsAssertion]
/// public static bool IsCustomAssertion(this int value, [CallerArgumentExpression("value")] string? expr = null) =>
///		Check.That(value > 0).Unless(value, $"in '{expr}' is not positive");
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class IsAssertionAttribute : Attribute;

/// <summary>
/// Mark a custom assertions class with this attribute to enable proper code line detection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IsAssertionsAttribute : Attribute;