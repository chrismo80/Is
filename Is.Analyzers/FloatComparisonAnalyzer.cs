using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Is.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FloatComparisonAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "IS001";

    private static readonly LocalizableString Title = "Use Is() instead of IsExactly() for floating-point comparisons";
    private static readonly LocalizableString MessageFormat = "Use Is() instead of IsExactly() for floating-point comparisons. Is() uses a tolerance-based comparison suitable for float/double.";
    private static readonly LocalizableString Description = "IsExactly() performs exact equality comparison which is unreliable for floating-point types due to precision issues. Use Is() which provides tolerance-based comparison.";

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

        if (memberAccess.Name.Identifier.Text != "IsExactly")
            return;

        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
            return;

        // Check if first argument is float or double
        if (methodSymbol.Parameters.Length == 0)
            return;

        var firstParamType = methodSymbol.Parameters[0].Type;
        if (firstParamType.SpecialType != SpecialType.System_Single && 
            firstParamType.SpecialType != SpecialType.System_Double)
            return;

        // Check if it's from the "Is" class
        if (methodSymbol.ContainingType.Name != "Is")
            return;

        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
