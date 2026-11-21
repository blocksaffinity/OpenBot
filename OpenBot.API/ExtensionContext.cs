using OpenBot.API.Extensions;
using OpenBot.API.Modules;

namespace OpenBot.API;

/// <summary>
/// Prodives useful context for an extension
/// </summary>
public sealed class ExtensionContext : ModuleContext
{
    public ExtensionMetadata Metadata { get; }
    public ExtensionContext(
        string name,
        ILogger logger,
        IModuleManager modules,
        IConfigurationModule configurationProvider,
        ExtensionMetadata meta
    )
            : 
    base(
        name,
        logger,
        modules,
        configurationProvider
    )

    {
        Metadata = meta;
    }
}