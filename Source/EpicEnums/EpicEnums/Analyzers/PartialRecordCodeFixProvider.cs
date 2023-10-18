using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace EpicEnums.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PartialRecordCodeFixProvider))]
[Shared]
public class PartialRecordCodeFixProvider : CodeFixProvider
{

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("EE0001");

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<RecordDeclarationSyntax>().First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Make record partial",
                createChangedDocument: c => MakeRecordPartialAsync(context.Document, declaration, c),
                equivalenceKey: "Make record partial"),
            diagnostic);
    }

    private async Task<Document> MakeRecordPartialAsync(Document document, RecordDeclarationSyntax recordDeclaration, CancellationToken cancellationToken)
    {
        var newModifiers = recordDeclaration.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        var newRecordDeclaration = recordDeclaration.WithModifiers(newModifiers);
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root.ReplaceNode(recordDeclaration, newRecordDeclaration);
        return document.WithSyntaxRoot(newRoot);
    }
}
