namespace Is.Core;

/// <summary>
/// Mark custom assertions with this attribute to enable proper code line detection.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// [IsExtension]
/// public static bool IsCustomAssertion(this bool value, [CallerArgumentExpression("value")] string? expr = null) =>
///		Check.That(value).Unless($"{expr} is wrong");
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class IsExtensionAttribute : Attribute;