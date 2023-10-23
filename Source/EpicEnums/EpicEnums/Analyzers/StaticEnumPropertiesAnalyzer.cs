using EpicEnums.CodeFix;
using EpicEnums.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Composition;
using System.Data;

namespace EpicEnums.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticEnumPropertiesCodeFixProvider)), Shared]
public class StaticEnumPropertiesAnalyzer : DiagnosticAnalyzer
{
    internal const string ErrorId = "EE0002";
    readonly DiagnosticDescriptor _enumPropertiesShouldBeStaticDescriptor = new(
               id: ErrorId,
               title: "EpicEnums",
               messageFormat: "EpicEnums: Property '{0}' of type '{1}' should be marked as static",
               category: "EpicEnums",
               DiagnosticSeverity.Error,
               isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_enumPropertiesShouldBeStaticDescriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
    }

    private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        //#if DEBUG
        //        if (!System.Diagnostics.Debugger.IsAttached)
        //        {
        //            System.Diagnostics.Debugger.Launch();
        //        }
        //#endif
        // Check if the enclosing type inherits from EpicEnum<T>
        if (propertyDeclaration.Parent is RecordDeclarationSyntax recordDeclaration &&
            recordDeclaration.BaseList is not null)
        {
            foreach (var baseType in recordDeclaration.BaseList.Types)
            {
                var typeSymbol = context.SemanticModel.GetTypeInfo(baseType.Type).Type as INamedTypeSymbol;

                if (typeSymbol?.ConstructedFrom.Name == "EpicEnum" &&
                    typeSymbol.TypeArguments.Length == 1 &&
                    typeSymbol.TypeArguments[0].Name == propertyDeclaration.Type.ToString())
                {
                    // Check if the property is not static
                    if (!propertyDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        var diagnostic = Diagnostic.Create(_enumPropertiesShouldBeStaticDescriptor, propertyDeclaration.GetLocation(), propertyDeclaration.Identifier.Text, propertyDeclaration.Type.ToString());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
