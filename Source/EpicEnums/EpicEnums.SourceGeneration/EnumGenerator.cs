using EpicEnums.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace EpicEnums.SourceGeneration;


[Generator]
public class EnumGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable S125 // Sections of code should not be commented out
        //#if DEBUG
        //        if (!System.Diagnostics.Debugger.IsAttached)
        //        {
        //            System.Diagnostics.Debugger.Launch();
        //        }
        //#endif
#pragma warning restore S125 // Sections of code should not be commented out
#pragma warning restore IDE0079 // Remove unnecessary suppression
        IncrementalValuesProvider<RecordDeclarationSyntax> epicEnumDeclarations = context.SyntaxProvider
           .CreateSyntaxProvider(
               predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
               transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EnumExtensions] attribute
           .Where(static m => m is not null)!; // filter out attributed enums that we don't care about


        IncrementalValueProvider<(Compilation, ImmutableArray<RecordDeclarationSyntax>)> compilationAndEnums
           = context.CompilationProvider.Combine(epicEnumDeclarations.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(compilationAndEnums,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));

    }

    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is RecordDeclarationSyntax m
            && m.BaseList?.Types.Count > 0;

    static RecordDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var recordDeclarationSyntax = (RecordDeclarationSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetDeclaredSymbol(recordDeclarationSyntax) is not INamedTypeSymbol recordSymbol)
        {
            // weird, we couldn't get the symbol, ignore it
            return null;
        }
        else if (recordSymbol.IsDerivedFromType("EpicEnum"))
        {
            return recordDeclarationSyntax;
        }
        // we didn't find the attribute we were looking for
        return null;
    }

    static void Execute(Compilation compilation, ImmutableArray<RecordDeclarationSyntax> epicEnums, SourceProductionContext context)
    {
        if (epicEnums.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        IEnumerable<RecordDeclarationSyntax> distinctEnums = epicEnums.Distinct();

        // Convert each EnumDeclarationSyntax to an EnumToGenerate
        List<EnumToGenerate> enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);


        foreach (var enumToGenerate in enumsToGenerate)
        {
            string source = GenerateSource(enumToGenerate);
            context.AddSource($"{enumToGenerate.Name}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static string GenerateSource(EnumToGenerate enumToGenerate)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {enumToGenerate.Namespace};");
        sb.AppendLine($"{(enumToGenerate.IsPublic ? "public" : "internal")} enum {enumToGenerate.Name}Enum");
        sb.AppendLine("{");

        foreach (var property in enumToGenerate.Values)
        {
            sb.AppendLine("    " + property + ",");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<RecordDeclarationSyntax> epicEnums, CancellationToken ct)
    {
        // Create a list to hold our output
        var enumsToGenerate = new List<EnumToGenerate>();
        // Get the semantic representation of our marker attribute 
        INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName("EpicEnums.EpicEnum");

        if (enumAttribute == null)
        {
            // If this is null, the compilation couldn't find the marker attribute type
            // which suggests there's something very wrong! Bail out..
            return enumsToGenerate;
        }

        foreach (RecordDeclarationSyntax recordDeclarationSyntax in epicEnums)
        {
            // stop if we're asked to
            ct.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            SemanticModel semanticModel = compilation.GetSemanticModel(recordDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(recordDeclarationSyntax) is not INamedTypeSymbol recordSymbol)
            {
                // something went wrong, bail out
                continue;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            string recordName = recordSymbol.ToString();

            var baseType = recordSymbol.BaseType;
            var enumType = baseType!.IsGenericType ? baseType.TypeArguments.First() : baseType.BaseType!.TypeArguments.First();

            var properties = recordSymbol.GetMembers().OfType<IPropertySymbol>()
                                .Where(m => SymbolEqualityComparer.Default.Equals(m.Type, enumType))
                                .Where(x => x.Name != "this[]");
            var members = properties.Select(x => x.Name).ToList();

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new EnumToGenerate(recordSymbol.Name, members, enumType.ToString(), recordSymbol.ContainingNamespace.ToString(), recordSymbol.DeclaredAccessibility == Accessibility.Public));
        }

        return enumsToGenerate;
    }

}

public readonly struct EnumToGenerate
{
    public readonly string Name;
    public readonly string Namespace;
    public readonly string Type;
    public readonly List<string> Values;
    public readonly bool IsPublic;

    public EnumToGenerate(string name, List<string> values, string type, string @namespace, bool isPublic)
    {
        Name = name;
        Values = values;
        Type = type;
        Namespace = @namespace;
        IsPublic = isPublic;
    }
}
