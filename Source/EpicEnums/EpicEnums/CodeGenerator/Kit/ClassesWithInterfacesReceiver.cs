namespace EpicEnums.CodeGenerator.Kit;

using Microsoft.CodeAnalysis;

public class ClassesWithInterfacesReceiver : SyntaxReceiver
{
    private readonly string _implementedInterface;
    public ClassesWithInterfacesReceiver(string implementedInterface) => this._implementedInterface = implementedInterface;

    public override bool CollectClassSymbol { get; } = true;

    protected override bool ShouldCollectClassSymbol(INamedTypeSymbol classSymbol)
        => classSymbol.IsImplements(_implementedInterface);
}
