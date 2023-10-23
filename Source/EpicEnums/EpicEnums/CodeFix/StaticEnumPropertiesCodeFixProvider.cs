using EpicEnums.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Text;

namespace EpicEnums.CodeFix;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StaticEnumPropertiesCodeFixProvider)), Shared]
internal class StaticEnumPropertiesCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(StaticEnumPropertiesAnalyzer.ErrorId);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the property declaration identified by the diagnostic.
        var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make property static",
                createChangedDocument: c => MakePropertyStaticAsync(context.Document, declaration, c),
                equivalenceKey: "MakePropertyStatic"),
            diagnostic);
    }

    private async Task<Document> MakePropertyStaticAsync(Document document, PropertyDeclarationSyntax propertyDecl, CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var staticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword);

        // Add the static modifier to the property
        editor.SetModifiers(propertyDecl, editor.Generator.GetModifiers(propertyDecl).WithIsStatic(true));

        return editor.GetChangedDocument();
    }
}
