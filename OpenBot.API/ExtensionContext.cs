using OpenBot.API.Enums;
using OpenBot.API.Extensions;
using OpenBot.API.Modules;

namespace OpenBot.API;

/// <summary>
/// Prodives useful context for an extension
/// </summary>
public sealed class ExtensionContext
{
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The extension's metadata (from its extension.json).
    /// </summary>
    public ExtensionMetadata Metadata { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The name of the extension.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The ILogger instance associated with the extension.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The IModuleManager for OpenBot to interact with modules.
    /// </summary>
    public IModuleManager Modules { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The globally registered IExtensionsManagerModule module.
    /// </summary>
    public IExtensionsManagerModule Extensions { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The globally registered IDiscordModule module.
    /// </summary>
    public IDiscordModule DiscordModule { get; }
    
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// The globally registered IConfigurationModule module.
    /// </summary>
    public IConfigurationModule ConfigurationModule { get; }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the status of any desired module by name.
    /// </summary>
    /// <param name="moduleName">The name of the module</param>
    /// <returns>The module's status, or null if the module is not registered</returns>
    public ModuleStatus? GetModuleStatus(string moduleName)
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
    public ModuleStatus GetModuleStatus<T>() where T : class, IModule
    {
        return this.Modules.GetOrThrow<T>().Status;
    }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the status of any desired extension by name
    /// </summary>
    /// <param name="extensionName">The name of the extension</param>
    /// <returns>The extension's status, or null if the extension is not registered</returns>
    public ExtensionStatus? GetStatus(string extensionName)
    {
        return this.Extensions.GetOrNull(extensionName)?.Status;
    }

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the status of the desired extension by its type
    /// </summary>
    /// <typeparam name="T">The type of the desired extension</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if an extension of the given type does not exist</exception>
    /// <returns>The extension's status, or throws InvalidOperationException if not registered</returns>
    public ExtensionStatus GetStatus<T>() where T : Extension
    {
        return this.Extensions.GetOrThrow<T>().Status;
    }

    public ExtensionContext(
        string name,
        ILogger logger,
        IModuleManager modules,
        ExtensionMetadata meta)
    {
        Name = name;
        Logger = logger;
        Modules = modules;
        Metadata = meta;

        Extensions = Modules.GetOrThrow<IExtensionsManagerModule>();
        DiscordModule = Modules.GetOrThrow<IDiscordModule>();
        ConfigurationModule = Modules.GetOrThrow<IConfigurationModule>();
    }
}