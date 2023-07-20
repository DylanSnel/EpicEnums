namespace EpicEnums.CodeGenerator.Kit;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

public class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<IMethodSymbol> Methods { get; } = new();
    public List<IFieldSymbol> Fields { get; } = new();
    public List<IPropertySymbol> Properties { get; } = new();
    public List<INamedTypeSymbol> Classes { get; } = new();

    public virtual bool CollectMethodSymbol { get; } = false;
    public virtual bool CollectFieldSymbol { get; } = false;
    public virtual bool CollectPropertySymbol { get; } = false;
    public virtual bool CollectClassSymbol { get; } = false;

    /// <inheritdoc/>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        switch (context.Node)
        {
            case MethodDeclarationSyntax methodDeclarationSyntax:
                OnVisitMethodDeclaration(methodDeclarationSyntax, context.SemanticModel);
                break;
            case PropertyDeclarationSyntax propertyDeclarationSyntax:
                OnVisitPropertyDeclaration(propertyDeclarationSyntax, context.SemanticModel);
                break;
            case FieldDeclarationSyntax fieldDeclarationSyntax:
                OnVisitFieldDeclaration(fieldDeclarationSyntax, context.SemanticModel);
                break;
            case ClassDeclarationSyntax classDeclarationSyntax:
                OnVisitClassDeclaration(classDeclarationSyntax, context.SemanticModel);
                break;
        }
    }

    protected virtual void OnVisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, SemanticModel model)
    {
        if (!CollectMethodSymbol)
        {
            return;
        }

        if (!ShouldCollectMethodDeclaration(methodDeclarationSyntax))
        {
            return;
        }

        var methodSymbol = model.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol;
        if (methodSymbol is null)
        {
            return;
        }

        if (!ShouldCollectMethodSymbol(methodSymbol))
        {
            return;
        }

        Methods.Add(methodSymbol);
    }

    protected virtual bool ShouldCollectMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax)
        => true;

    protected virtual bool ShouldCollectMethodSymbol(IMethodSymbol methodSymbol)
        => true;

    protected virtual void OnVisitFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel model)
    {
        if (!CollectFieldSymbol)
        {
            return;
        }

        if (!ShouldCollectFieldDeclaration(fieldDeclarationSyntax))
        {
            return;
        }

        var fieldSymbol = model.GetDeclaredSymbol(fieldDeclarationSyntax) as IFieldSymbol;
        if (fieldSymbol == null)
        {
            return;
        }

        if (!ShouldCollectFieldSymbol(fieldSymbol))
        {
            return;
        }

        Fields.Add(fieldSymbol);
    }

    protected virtual bool ShouldCollectFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax)
        => true;

    protected virtual bool ShouldCollectFieldSymbol(IFieldSymbol fieldSymbol)
        => true;

    protected virtual void OnVisitPropertyDeclaration(PropertyDeclarationSyntax propertyDeclarationSyntax, SemanticModel model)
    {
        if (!CollectPropertySymbol)
        {
            return;
        }

        if (!ShouldCollectPropertyDeclaration(propertyDeclarationSyntax))
        {
            return;
        }

        var propertySymbol = model.GetDeclaredSymbol(propertyDeclarationSyntax) as IPropertySymbol;
        if (propertySymbol == null)
        {
            return;
        }

        if (!ShouldCollectPropertySymbol(propertySymbol))
        {
            return;
        }

        Properties.Add(propertySymbol);
    }

    protected virtual bool ShouldCollectPropertyDeclaration(PropertyDeclarationSyntax propertyDeclarationSyntax)
        => true;

    protected virtual bool ShouldCollectPropertySymbol(IPropertySymbol propertySymbol)
        => true;

    protected virtual void OnVisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel model)
    {
        if (!CollectClassSymbol)
        {
            return;
        }

        if (!ShouldCollectClassDeclaration(classDeclarationSyntax))
        {
            return;
        }

        var classSymbol = model.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
        if (classSymbol == null)
        {
            return;
        }

        if (!ShouldCollectClassSymbol(classSymbol))
        {
            return;
        }

        Classes.Add(classSymbol);
    }

    protected virtual bool ShouldCollectClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax)
        => true;

    protected virtual bool ShouldCollectClassSymbol(INamedTypeSymbol classSymbol)
        => true;
}
