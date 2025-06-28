namespace Is.Core;

/// <summary>
/// Mark custom assertion methods with this attribute to enable proper code line detection.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// [IsAssertion]
/// public static bool IsCustomAssertion(this bool value, [CallerArgumentExpression("value")] string? expr = null) =>
///		Check.That(value).Unless($"{expr} is wrong");
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class IsAssertionAttribute : Attribute;

/// <summary>
/// Mark a custom assertions class with this attribute to enable proper code line detection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IsAssertionsAttribute : Attribute;