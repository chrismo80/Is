using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Is.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullCheckOnNonNullableAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "IS002";

    private static readonly LocalizableString Title = "IsNull check on non-nullable type";
    private static readonly LocalizableString MessageFormat = "IsNull() check on non-nullable type '{0}' is always false. The type is not nullable.";
    private static readonly LocalizableString Description = "Checking IsNull() on a non-nullable type will always fail. Remove the null check or make the type nullable if null values are expected.";

    private const string Category = "Correctness";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return;

        if (memberAccess.Name.Identifier.Text != "IsNull")
            return;

        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
            return;

        // Check if it's from the "Is" class
        if (methodSymbol.ContainingType.Name != "Is")
            return;

        // Get the type argument (generic type)
        if (methodSymbol.TypeArguments.Length == 0)
            return;

        var typeArg = methodSymbol.TypeArguments[0];
        
        // Check if type is non-nullable value type or non-nullable reference type
        if (typeArg.IsValueType && typeArg.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T)
        {
            var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), typeArg.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
