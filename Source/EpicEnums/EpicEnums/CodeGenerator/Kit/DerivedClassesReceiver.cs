namespace EpicEnums.CodeGenerator.Kit;

using Microsoft.CodeAnalysis;

public class DerivedClassesReceiver : SyntaxReceiver
{
    private readonly string _baseTypeName;
    public DerivedClassesReceiver(string baseTypeName) => this._baseTypeName = baseTypeName;

    public override bool CollectClassSymbol { get; } = true;

    protected override bool ShouldCollectClassSymbol(INamedTypeSymbol classSymbol)
        => classSymbol.IsDerivedFromType(_baseTypeName);
}
