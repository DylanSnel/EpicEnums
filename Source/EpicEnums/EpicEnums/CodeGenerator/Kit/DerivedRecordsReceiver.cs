namespace EpicEnums.CodeGenerator.Kit;

using Microsoft.CodeAnalysis;

public class DerivedRecordsReceiver : SyntaxReceiver
{
    private readonly string _baseTypeName;
    public DerivedRecordsReceiver(string baseTypeName) => this._baseTypeName = baseTypeName;

    public override bool CollectRecordSymbol { get; } = true;

    protected override bool ShouldCollectRecordSymbol(INamedTypeSymbol recordSymbol)
        => recordSymbol.IsDerivedFromType(_baseTypeName);
}
