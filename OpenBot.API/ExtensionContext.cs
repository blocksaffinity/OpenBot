using OpenBot.API.Modules;

namespace OpenBot.API;

public sealed class ExtensionContext : ModuleContext
{
    public ExtensionContext(
        string name,
        ILogger logger,
        IModuleProvider modules,
        IConfigurationProvider configurationProvider)
        : base(name, logger, modules, configurationProvider)
    {
    }
}