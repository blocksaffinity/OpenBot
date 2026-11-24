using OpenBot.API.Modules;

namespace OpenBot.API.Extensions;

public interface IExtensionsManagerModule : IModule
{
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Returns the ExtensionMetadata for the given Extension, or null if not found.
    /// </summary>
    ExtensionMetadata? GetExtensionMetadata(Extension extension);

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Returns the ExtensionMetadata for the given extension name, or null if not found.
    /// </summary>
    ExtensionMetadata? GetExtensionMetadata(string extensionName);


    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a list of registered extennsions.
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<Extension> GetExtensions();


    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves the number of registered extensions.
    /// </summary>
    int GetExtensionsCount();


    /// <summary>
    /// Retrieves an Extension of the given name, or null if one is not registered
    /// </summary>
    /// <param name="name">The name of the desired Extension</param>
    /// <returns>The desired Extension, or null if not registered</returns>
    public Extension? GetOrNull(string name);

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a registered extension of the given type, or null if one is not found.
    /// </summary>
    /// <typeparam name="T">The type of the desired extension</typeparam>
    /// <returns>A registered Extension of the given type, or null if no such extension is registered.</returns>
    public T? GetOrNull<T>() where T : Extension;

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Retrieves a registered extension of the given type, or throws InvalidOperationException if such extension is not registered.
    /// </summary>
    /// <typeparam name="T">The type of the desired extension</typeparam>
    /// <exception cref="InvalidOperationException">Thrown if a extension of the given type does not exist</exception>
    /// <returns>A registered Extension of the given type.</returns>
    public T GetOrThrow<T>() where T : Extension;

    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Determines whether a extension of the given type is registered.
    /// </summary>
    /// <typeparam name="T">The type of the extension to query</typeparam>
    /// <returns>True if the extension is registered, otherwise false.</returns>
    public bool IsRegistered<T>() where T : Extension;
    
    /// <summary>
    /// [0.1.0-beta]<br/>
    /// Determines whether a extension of the given type is started.
    /// </summary>
    /// <typeparam name="T">The type of the extension to query</typeparam>
    /// <returns>True if the extension is started, otherwise false.</returns>
    public bool IsStarted<T>() where T : Extension;
}