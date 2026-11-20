using OpenBot.API.Modules;

namespace OpenBot.API;

/// <summary>
/// Prodives useful context for an extension
/// </summary>
public sealed class ExtensionContext : ModuleContext
{
    public ExtensionContext(
        string name,
        ILogger logger,
        IModuleManager modules,
        IConfigurationModule configurationProvider
    )
            : 
    base(
        name,
        logger,
        modules,
        configurationProvider
    )
    
    {
    }
}