using OpenBot.API.Enums;
using OpenBot.API.Extensions;
using OpenBot.API.Modules;

namespace OpenBot.API;

public class ModuleContext
{
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The name of the module.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The ILogger instance associated with the module.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The IModuleManager for OpenBot to interact with modules.
    /// </summary>
    public IModuleManager Modules { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The globally registered IDiscordModule module.
    /// </summary>
    public IDiscordModule? DiscordModule { get; set; }
    
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The globally registered IConfigurationModule module.
    /// </summary>
    public IConfigurationModule? ConfigurationModule { get; set; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the status of any desired module by name.
    /// </summary>
    /// <param name="moduleName">The name of the module</param>
    /// <returns>The module's status, or null if the module is not registered</returns>
    public ModuleStatus? GetStatus(string moduleName)
    {
        return this.Modules.GetOrNull(moduleName)?.Status;
    }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the status of the desired module by its type.
    /// </summary>
    /// <typeparam name="T">The type of the desired module</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if a module of the given type does not exist</exception>
    /// <returns>The module's status, or throws InvalidOperationException if not registered</returns>
    public ModuleStatus GetStatus<T>() where T : class, IModule
    {
        return this.Modules.GetOrThrow<T>().Status;
    }

    public ModuleContext(
        string name,
        ILogger logger,
        IModuleManager modules)
    {
        Name = name;
        Logger = logger;
        Modules = modules;

        DiscordModule = Modules.GetOrNull<IDiscordModule>();
        ConfigurationModule = Modules.GetOrNull<IConfigurationModule>();
    }
}