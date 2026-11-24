namespace OpenBot.API.Modules;

public interface IModuleManager
{
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a list of registered modules.
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<IModule> GetModules();

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the number of registered modules.
    /// </summary>
    int GetModulesCount();

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves an IModule of the given name, or null if one is not registered
    /// </summary>
    /// <param name="name">The name of the desired IModule</param>
    /// <returns>The desired IModule, or null if not registered</returns>
    public IModule? GetOrNull(string name);

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a registered module of the given type, or null if one is not found.
    /// </summary>
    /// <typeparam name="T">The type of the desired module</typeparam>
    /// <returns>A registered Module of the given type, or null if no such module is registered.</returns>
    public T? GetOrNull<T>() where T : class, IModule;

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a registered module of the given type, or throws InvalidOperationException if such module is not registered.
    /// </summary>
    /// <typeparam name="T">The type of the desired module</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if a module of the given type does not exist</exception>
    /// <returns>A registered Module of the given type.</returns>
    public T GetOrThrow<T>() where T : class, IModule;

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Determines whether a module of the given type is registered.
    /// </summary>
    /// <typeparam name="T">The type of the module to query</typeparam>
    /// <returns>True if the module is registered, otherwise false.</returns>
    public bool IsRegistered<T>() where T : class, IModule;
    
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Determines whether a module of the given type is started.
    /// </summary>
    /// <typeparam name="T">The type of the module to query</typeparam>
    /// <returns>True if the module is started, otherwise false.</returns>
    public bool IsStarted<T>() where T : class, IModule;
}