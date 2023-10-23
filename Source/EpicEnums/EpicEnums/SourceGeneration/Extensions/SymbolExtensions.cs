namespace EpicEnums.SourceGeneration.Extensions;

using Microsoft.CodeAnalysis;
using System.Linq;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string atrributeName)
    {
        return symbol.GetAttributes()
            .Any(_ => _.AttributeClass?.Name == atrributeName);
    }

    public static AttributeData? FindAttribute(this ISymbol symbol, string atrributeName)
    {
        return symbol.GetAttributes()
            .FirstOrDefault(_ => _.AttributeClass?.Name == atrributeName);
    }

    public static bool IsDirectlyDerivedDrom(this INamedTypeSymbol symbol, string typeName)
    {
        if (symbol.BaseType == null)
        {
            return false;
        }
        if (symbol.BaseType.Name == typeName)
        {
            return true;
        }
        return false;
    }

    public static bool IsDerivedFromType(this INamedTypeSymbol symbol, string typeName)
    {
        if (symbol.Name == typeName)
        {
            return true;
        }

        if (symbol.BaseType == null)
        {
            return false;
        }

        return symbol.BaseType.IsDerivedFromType(typeName);
    }

    public static bool IsImplements(this INamedTypeSymbol symbol, string typeName)
    {
        return symbol.AllInterfaces.Any(_ => _.Name == typeName);
    }
}
