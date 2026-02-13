using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Is.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingUsingOnAssertionContextAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "IS004";

    private static readonly LocalizableString Title = "AssertionContext should be used with 'using' statement";
    private static readonly LocalizableString MessageFormat = "AssertionContext implements IDisposable and should be used within a 'using' statement to ensure proper cleanup.";
    private static readonly LocalizableString Description = "AssertionContext is IDisposable and manages resources that need to be released. Always use it within a 'using' statement or declaration.";

    private const string Category = "Reliability";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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

        if (memberAccess.Name.Identifier.Text != "CreateContext")
            return;

        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
            return;

        // Check if it's from the "Is" class and returns AssertionContext
        if (methodSymbol.ContainingType.Name != "Is")
            return;

        var returnType = methodSymbol.ReturnType;
        if (returnType.Name != "AssertionContext")
            return;

        // Check if parent is a using statement or using declaration
        if (IsWithinUsing(invocation))
            return;

        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsWithinUsing(InvocationExpressionSyntax invocation)
    {
        var parent = invocation.Parent;
        
        while (parent != null)
        {
            // Check for using statement: using (var x = ...)
            if (parent is UsingStatementSyntax usingStatement)
            {
                // Check if our invocation is part of the declaration or expression
                return true;
            }
            
            // Check for using declaration: using var x = ...
            if (parent is LocalDeclarationStatementSyntax localDecl)
            {
                if (localDecl.UsingKeyword.Kind() == SyntaxKind.UsingKeyword)
                    return true;
            }

            // If we hit a method declaration, statement (not using), or member, stop
            if (parent is MethodDeclarationSyntax ||
                parent is StatementSyntax ||
                parent is MemberDeclarationSyntax)
            {
                break;
            }

            parent = parent.Parent;
        }

        return false;
    }
}
