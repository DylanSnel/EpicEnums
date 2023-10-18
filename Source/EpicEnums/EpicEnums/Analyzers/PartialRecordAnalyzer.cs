using EpicEnums.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace EpicEnums.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PartialRecordAnalyzer : DiagnosticAnalyzer
{
    readonly DiagnosticDescriptor _recordShouldBePartialDescriptor = new(
               id: "EE0001",
               title: "EpicEnums",
               messageFormat: "EpicEnums: Record {0} inherits {1} should be marked partial",
               category: "EpicEnums",
               DiagnosticSeverity.Error,
               isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_recordShouldBePartialDescriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeRecordDeclaration, SyntaxKind.RecordDeclaration);
    }

    private void AnalyzeRecordDeclaration(SyntaxNodeAnalysisContext context)
    {
        var recordDeclaration = (RecordDeclarationSyntax)context.Node;
        var recordSymbol = context.SemanticModel.GetDeclaredSymbol(recordDeclaration);
        if (recordSymbol is null)
        {
            return;
        }
        var recordName = recordSymbol.Name;
        var fromEpicEnum = recordSymbol.IsDirectlyDerivedDrom(nameof(EpicEnum));
        var fromEpicEnumValue = recordSymbol.IsImplements(nameof(IEpicEnumValue));
        if (!fromEpicEnum && !fromEpicEnumValue)
        {
            return;
        }

        if (recordDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }
        string baseType = fromEpicEnum ? nameof(EpicEnum) : nameof(IEpicEnumValue);

        context.ReportDiagnostic(Diagnostic.Create(_recordShouldBePartialDescriptor, context.Node.GetLocation(), recordName, baseType));
    }
}
