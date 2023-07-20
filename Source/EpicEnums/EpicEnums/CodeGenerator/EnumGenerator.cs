using EpicEnums.CodeGenerator.Kit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace EpicEnums.CodeGenerator;


[Generator]
public class EnumGenerator : ISourceGenerator
{
    readonly SyntaxReceiver _syntaxReceiver = new DerivedClassesReceiver("EpicEnum");

    public void Initialize(GeneratorInitializationContext context)
    {
#pragma warning disable S125 // Sections of code should not be commented out
        //#if DEBUG
        //        if (!Debugger.IsAttached)


        //        {
        //            Debugger.Launch();
        //        }
        //#endif
        context.RegisterForSyntaxNotifications(() => _syntaxReceiver);
#pragma warning restore S125 // Sections of code should not be commented out
    }

    public void Execute(GeneratorExecutionContext context)
    {

        Debug.WriteLine("Hello from the source generator!");

        // Retrieve the populated receiver
        if (!(context.SyntaxContextReceiver is SyntaxReceiver))
        {
            return;
        }

        foreach (INamedTypeSymbol c in this._syntaxReceiver.Classes)
        {
            var baseType = c.BaseType;
            var enumType = baseType!.IsGenericType ? baseType.TypeArguments.First() : baseType.BaseType!.TypeArguments.First();

            var properties = c.GetMembers().OfType<IPropertySymbol>()
                                .Where(m => SymbolEqualityComparer.Default.Equals(m.Type, enumType));

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {c.ContainingNamespace};");
            sb.AppendLine("public enum " + c.Name + "Enum");
            sb.AppendLine("{");

            foreach (var p in properties)
            {
                sb.AppendLine("    " + p.Name + ",");
            }

            sb.AppendLine("}");

            context.AddSource($"{c.ContainingNamespace}.{c.Name}Enum_{Guid.NewGuid()}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

    }
}
