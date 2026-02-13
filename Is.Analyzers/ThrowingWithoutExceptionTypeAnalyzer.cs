using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Is.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ThrowingWithoutExceptionTypeAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "IS003";

    private static readonly LocalizableString Title = "IsThrowing without exception type check";
    private static readonly LocalizableString MessageFormat = "IsThrowing() without specifying the expected exception type may hide unexpected exceptions. Consider using IsThrowing<TException>().";
    private static readonly LocalizableString Description = "Using IsThrowing() without a type parameter catches any exception, which may mask bugs. Specify the expected exception type for more precise testing.";

    private const string Category = "Reliability";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Info,
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

        if (memberAccess.Name.Identifier.Text != "IsThrowing")
            return;

        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
            return;

        // Check if it's from the "Is" class
        if (methodSymbol.ContainingType.Name != "Is")
            return;

        // Check if it's the non-generic version (no type arguments)
        if (methodSymbol.TypeArguments.Length > 0)
            return;

        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
