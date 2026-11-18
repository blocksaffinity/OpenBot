using OpenBot.API.Enums;
using OpenBot.API.Modules;

namespace OpenBot.API;

public class ModuleContext
{
    public string Name { get; }
    public ILogger Logger { get; }
    public IModuleProvider Modules { get; }
    public IConfigurationProvider? Configuration { get; }
    public ModuleStatus? GetStatus(string moduleName)
    {
        foreach (Module module in Modules.GetModules().Cast<Module>())
        {
            if (module.Name == moduleName)
            {
                return module.Status;
            }
        }
        return null;
    }

    public ModuleContext(
        string name,
        ILogger logger,
        IModuleProvider modules,
        IConfigurationProvider? configuration)
    {
        Name = name;
        Logger = logger;
        Modules = modules;
        Configuration = configuration;
    }
}