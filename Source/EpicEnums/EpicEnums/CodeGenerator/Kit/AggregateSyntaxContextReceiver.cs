namespace EpicEnums.CodeGenerator.Kit;

using Microsoft.CodeAnalysis;

/// <summary>
/// Provides syntax context receivier which delegates work to multiple receivers.
/// </summary>
public class AggregateSyntaxContextReceiver : ISyntaxContextReceiver
{
    public AggregateSyntaxContextReceiver(params ISyntaxContextReceiver[] receivers) => Receivers = receivers;

    public ISyntaxContextReceiver[] Receivers { get; }

    /// <inheritdoc/>
    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        foreach (var receiver in Receivers)
        {
            receiver.OnVisitSyntaxNode(context);
        }
    }
}
