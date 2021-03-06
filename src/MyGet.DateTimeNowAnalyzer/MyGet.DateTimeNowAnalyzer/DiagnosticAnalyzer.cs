using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyGet.DateTimeNowAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MyGetDateTimeNowAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MyGetDateTimeNowAnalyzer";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIdentifierName, SyntaxKind.IdentifierName);
        }

        private void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
        {
            var identifierName = context.Node as IdentifierNameSyntax;
            if (identifierName != null)
            {
                // Find usages of "DateTime.Now"
                if (identifierName.Identifier.ValueText == "Now")
                {
                    var expression = ((MemberAccessExpressionSyntax)identifierName.Parent).Expression;
                    var memberSymbol = context.SemanticModel.GetSymbolInfo(expression).Symbol;

                    if (!memberSymbol?.ToString().StartsWith("System.DateTime") ?? true)
                    {
                        return;
                    }
                    else
                    {
                        // Produce a diagnostic.
                        var diagnostic = Diagnostic.Create(Rule, identifierName.Identifier.GetLocation(), identifierName);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
