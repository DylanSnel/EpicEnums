﻿using EpicEnums.SourceGeneration.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace EpicEnums.SourceGeneration;


[Generator]
internal class EnumGenerator : IIncrementalGenerator
{
    private const string Header = """
                                    //------------------------------------------------------------------------------
                                    // <auto-generated>
                                    //     This code was generated by the EpicEnums source generator
                                    //
                                    //     Changes to this file may cause incorrect behavior and will be lost if
                                    //     the code is regenerated.
                                    // </auto-generated>
                                    //------------------------------------------------------------------------------

                                    #nullable enable
                                    """;



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
            if (enumToGenerate.Values.Count < 1)
            {
                continue;
            }
            GenerateEnum(context, enumToGenerate);

            GenerateSourceEnumPartial(context, enumToGenerate);
            GenerateSourceEnumTypePartial(context, enumToGenerate);
        }
    }

    private static void GenerateSourceEnumTypePartial(SourceProductionContext context, EnumToGenerate enumToGenerate)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Header);
        sb.AppendLine(
           $$$""""
                using EpicEnums.Exceptions;

                namespace {{{enumToGenerate.Namespace}}};

                {{{(enumToGenerate.IsPublic ? "public" : "internal")}}} partial record {{{enumToGenerate.Type}}} 
                {
                    internal {{{enumToGenerate.EnumName}}}? {{{enumToGenerate.ValueName}}}
                    {
                        init
                        {
                            _{{{enumToGenerate.ValueName}}} = value;
                        }
                    }

                    private {{{enumToGenerate.EnumName}}}? _{{{enumToGenerate.ValueName}}};

                    public bool Is{{{enumToGenerate.EnumName}}}()
                    {
                        return _{{{enumToGenerate.ValueName}}} is not null;
                    }
                    public static implicit operator {{{enumToGenerate.EnumName}}}({{{enumToGenerate.Type}}} {{{enumToGenerate.Type.ToLower()}}})
                    {
                        return {{{enumToGenerate.Type.ToLower()}}}._{{{enumToGenerate.ValueName}}} ?? throw new UnsupportedValueException();
                    }

                    public static implicit operator {{{enumToGenerate.Type}}}({{{enumToGenerate.EnumName}}} {{{enumToGenerate.Type.ToLower()}}})
                    {
                        return {{{enumToGenerate.Name}}}.FromEnum({{{enumToGenerate.Type.ToLower()}}});
                    }

                    public static bool operator ==({{{enumToGenerate.Type}}} left, {{{enumToGenerate.EnumName}}} right)
                    {
                        return left._{{{enumToGenerate.ValueName}}} == right;
                    }
                    public static bool operator !=({{{enumToGenerate.Type}}} left, {{{enumToGenerate.EnumName}}} right)
                    {
                        return left._{{{enumToGenerate.ValueName}}} != right;
                    }
                }
                """");

        context.AddSource($"{enumToGenerate.Name}_{enumToGenerate.Type}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }


    private static void GenerateSourceEnumPartial(SourceProductionContext context, EnumToGenerate enumToGenerate)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Header);
        var enumerator = new StringBuilder();
        var constructor = new StringBuilder();
        var selectorSwitch = new StringBuilder();

        foreach (var property in enumToGenerate.Values)
        {
            enumerator.AppendLine($"yield return {property};");
            constructor.AppendLine($"{property} = {property} with {{ {enumToGenerate.ValueName} = {enumToGenerate.EnumName}.{property} }};");
            selectorSwitch.AppendLine($"{enumToGenerate.EnumName}.{property} => {property},");
        }
        selectorSwitch.AppendLine($"_ => throw new UnsupportedValueException($\"{{ {enumToGenerate.Type.ToLower()} }}\")");

        sb.AppendLine(
           $$$""""
                using System.Collections;
                using EpicEnums.Exceptions;

                namespace {{{enumToGenerate.Namespace}}};

                {{{(enumToGenerate.IsPublic ? "public" : "internal")}}} partial record {{{enumToGenerate.Name}}}: IEnumerable<{{{enumToGenerate.Type}}}> 
                {
                    static {{{enumToGenerate.Name}}}()
                    {
                        {{{constructor}}}
                    }

                    public {{{enumToGenerate.Type}}} this[{{{enumToGenerate.EnumName}}} {{{enumToGenerate.Type.ToLower()}}}]
                        => FromEnum({{{enumToGenerate.Type.ToLower()}}});

                    public static {{{enumToGenerate.Type}}} FromEnum({{{enumToGenerate.EnumName}}} {{{enumToGenerate.Type.ToLower()}}})
                        => {{{enumToGenerate.Type.ToLower()}}} switch
                            {
                                {{{selectorSwitch}}}
                            };


                    public static IEnumerable<{{{enumToGenerate.Type}}}> Enumerable()
                    {
                        {{{enumerator}}}
                    }

                    public static IEnumerable<{{{enumToGenerate.Type}}}> Values
                    {
                        get
                        {
                            return Enumerable();
                        }
                    }

                    public IEnumerator<{{{enumToGenerate.Type}}}> GetEnumerator()
                    {
                        return Enumerable().GetEnumerator();
                    }

                    IEnumerator IEnumerable.GetEnumerator()
                    {
                        return Enumerable().GetEnumerator();
                    }
                }
                """");

        context.AddSource($"{enumToGenerate.Name}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void GenerateEnum(SourceProductionContext context, EnumToGenerate enumToGenerate)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Header);
        sb.AppendLine($"namespace {enumToGenerate.Namespace};");
        sb.AppendLine($"{(enumToGenerate.IsPublic ? "public" : "internal")} enum {enumToGenerate.Name}Enum");
        sb.AppendLine("{");

        foreach (var property in enumToGenerate.Values)
        {
            sb.AppendLine("    " + property + ",");
        }

        //if (sb.Length > 0)
        //{
        //    // remove the last character which is the comma
        //    sb.Remove(sb.Length - 3, 1);
        //}
        sb.AppendLine("}");
        context.AddSource($"{enumToGenerate.Name}Enum.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
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
                                .Where(m => SymbolEqualityComparer.Default.Equals(m.Type, enumType));
            var members = properties.Select(x => x.Name).ToList();

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new EnumToGenerate(recordSymbol.Name, members, enumType.Name, recordSymbol.ContainingNamespace.ToString(), recordSymbol.DeclaredAccessibility == Accessibility.Public));
        }

        return enumsToGenerate;
    }

}

internal readonly struct EnumToGenerate
{
    public readonly string Name;
    public readonly string Namespace;
    public readonly string Type;
    public readonly List<string> Values;
    public readonly bool IsPublic;
    public readonly string EnumName;
    public readonly string ValueName;

    public EnumToGenerate(string name, List<string> values, string type, string @namespace, bool isPublic)
    {
        Name = name;
        EnumName = $"{name}Enum";
        ValueName = $"{name}Value";
        Values = values;
        Type = type;
        Namespace = @namespace;
        IsPublic = isPublic;
    }
}
