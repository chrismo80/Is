using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Is.Analyzers.Tests;

public class AnalyzerMetadataTests
{
    [Theory]
    [InlineData(typeof(FloatComparisonAnalyzer), "IS001", DiagnosticSeverity.Warning)]
    [InlineData(typeof(NullCheckOnNonNullableAnalyzer), "IS002", DiagnosticSeverity.Warning)]
    [InlineData(typeof(ThrowingWithoutExceptionTypeAnalyzer), "IS003", DiagnosticSeverity.Info)]
    [InlineData(typeof(MissingUsingOnAssertionContextAnalyzer), "IS004", DiagnosticSeverity.Warning)]
    public void Analyzer_HasCorrectIdAndSeverity(Type analyzerType, string expectedId, DiagnosticSeverity expectedSeverity)
    {
        var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType)!;
        
        Assert.Single(analyzer.SupportedDiagnostics);
        
        var descriptor = analyzer.SupportedDiagnostics[0];
        Assert.Equal(expectedId, descriptor.Id);
        Assert.Equal(expectedSeverity, descriptor.DefaultSeverity);
        Assert.True(descriptor.IsEnabledByDefault);
    }

    [Fact]
    public void FloatComparisonAnalyzer_TitleContainsExactly()
    {
        var analyzer = new FloatComparisonAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics[0];
        
        Assert.Contains("Exactly", descriptor.Title.ToString());
        Assert.Contains("float", descriptor.Description.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullCheckAnalyzer_DetectsValueTypes()
    {
        var analyzer = new NullCheckOnNonNullableAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics[0];
        
        Assert.Contains("non-nullable", descriptor.Description.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ThrowingAnalyzer_IsInfoSeverity()
    {
        var analyzer = new ThrowingWithoutExceptionTypeAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics[0];
        
        Assert.Equal(DiagnosticSeverity.Info, descriptor.DefaultSeverity);
        Assert.Contains("exception", descriptor.Description.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UsingAnalyzer_ReferencesDisposable()
    {
        var analyzer = new MissingUsingOnAssertionContextAnalyzer();
        var descriptor = analyzer.SupportedDiagnostics[0];
        
        Assert.Contains("using", descriptor.Title.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("IDisposable", descriptor.Description.ToString());
    }
}
