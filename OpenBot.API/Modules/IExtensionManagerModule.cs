using OpenBot.API.Extensions;

namespace OpenBot.API.Modules;

public interface IExtensionManagerModule
{
    /// <summary>
    /// Returns the ExtensionMetadata for the given Extension, or null if not found
    /// </summary>
    ExtensionMetadata? GetExtensionMetadata(Extension extension);

    /// <summary>
    /// Returns the ExtensionMetadata for the given extension name, or null if not found
    /// </summary>
    ExtensionMetadata? GetExtensionMetadata(string extensionName);

    /// <summary>
    /// Retrieves a list of currently registered extensions
    /// </summary>
    /// <returns>A read-only collection of registered extensions</returns>
    IReadOnlyCollection<Extension> GetExtensions();

    /// <summary>
    /// Gets the number of currently registered extensions
    /// </summary>
    /// <returns>The number of currently registered extensions</returns>
    int GetExtensionsCount();

    /// <summary>
    /// Gets a registered extension of the provided name
    /// </summary>
    /// <param name="name">The name of the desired extension</param>
    /// <returns>An Extension with a name which matches the provided name, or null if no extension was found</returns>
    Extension? Get(string name);

    /// <summary>
    /// Retrieves a registered extension of the provided type, or null if one is not registered
    /// </summary>
    /// <typeparam name="T">The type/class of the desired extension</typeparam>
    /// <returns>The requested Extension, or null if one is not found</returns>
    T? Get<T>() where T : Extension;
}