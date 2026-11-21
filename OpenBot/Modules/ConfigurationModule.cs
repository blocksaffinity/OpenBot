using YamlDotNet.Serialization;
using OpenBot.API.Modules;
using OpenBot.API;
using OpenBot.Utils;
using YamlDotNet.Core;
using OpenBot.Exceptions;

namespace OpenBot.Modules;

internal sealed class ConfigurationModule : Module, IConfigurationModule
{
    private readonly Dictionary<string, object> configurationCache = [];
    private readonly ISerializer serializer;
    private readonly IDeserializer deserializer;

    internal ConfigurationModule(ModuleContext context)
        : base(context)
    {
        serializer = new SerializerBuilder().Build();
        deserializer = new DeserializerBuilder().Build();
    }

    /// <summary>
    /// Retrieves the configuration path of the provided module
    /// </summary>
    /// <param name="moduleName">The name of the module</param>
    /// <returns>The full path to the configuration file for the module</returns>
    private string GetConfigPath(string moduleName)
    {
        string botRootPath = API.OpenBot.GetRootDirectory();

        // FIXME: for some reason, GetModules returns a list of extensions? maybe? maybe not???
        bool isExtension = this.Context.Modules.GetModules().Any(
            module =>
                module.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase)
        );

        // make sure its a module and not an extension
        if (!isExtension)
        {
            return Path.GetFullPath(Path.Combine(botRootPath, "ModuleConfig", moduleName, "config.yml"));
        }
        else
        {
            return Path.GetFullPath(Path.Combine(botRootPath, "ExtensionConfig", moduleName, "config.yml"));
        }
    }

    /// <summary>
    /// Retrieves the configuration associated with a module.
    /// </summary>
    /// <typeparam name="T">The expected configuration type</typeparam>
    /// <param name="moduleName">The name of the module requesting its configuration</param>
    /// <returns>The module's configuration instance</returns>
    /// <exception cref="InvalidOperationException">Thrown when the module already has a configuration registered with a different type</exception>
    /// <exception cref="ConfigurationException">Thrown when the module's configuration file cannot be deserialized</exception>
    public T GetConfig<T>(string moduleName) where T : class, new()
    {
        if (configurationCache.TryGetValue(moduleName, out var cachedConfig))
        {
            // It's in cache

            if (cachedConfig is T validTypeConfig)
            {
                // Return cached config
                return validTypeConfig;
            }
            // the cached config is of the WRONG type.
            throw new InvalidOperationException($"Configuration for '{moduleName}' is already loaded with a different type. Expected '{typeof(T)}'");
        }

        // It's not in cache

        string yamlPath = GetConfigPath(moduleName);

        T config;

        if (!File.Exists(yamlPath))
        {
            // Save default config to yaml
            config = new T();
            FileUtils.EnsureDirectoryExists(Path.GetDirectoryName(yamlPath));
            File.WriteAllText(yamlPath, serializer.Serialize(config));
        }
        else
        {
            string contents = File.ReadAllText(yamlPath);
            // Deserialize file contents into config variable
            // TODO: Deserialize<T> can return null, this must be handled
            try
            {
                config = deserializer.Deserialize<T>(contents);
            }
            catch (YamlException ex)
            {
                throw new ConfigurationException(moduleName, $"Failed to deserialize {Path.GetFileName(yamlPath)}, please check your configuration", ex);
            }
        }

        configurationCache.Add(moduleName, config);

        return config;
    }

    /// <summary>
    /// Saves the provided configuration to the disk and updates the internal cache.
    /// </summary>
    /// <typeparam name="T">The type of the config</typeparam>
    /// <param name="moduleName">The name of the module the configuration is associated with</param>
    /// <param name="config">The configuration itself</param>
    /// <exception cref="InvalidOperationException">Thrown when the module already has a configuration registered with a different type</exception>
    /// /// <exception cref="ConfigurationException">Thrown when the module's configuration file cannot be deserialized</exception>
    public void SaveConfig<T>(string moduleName, T config) where T : class
    {
        if (configurationCache.ContainsKey(moduleName))
        {
            if (configurationCache[moduleName] is not T)
            {
                throw new InvalidOperationException($"Configuration for '{moduleName}' is already loaded with a different type. Expected '{typeof(T)}'");
            }

            // Update cache
            configurationCache[moduleName] = config;
        }
        else
        {
            // Add to cache
            configurationCache.Add(moduleName, config);
        }

        string configPath = GetConfigPath(moduleName);
        FileUtils.EnsureDirectoryExists(Path.GetDirectoryName(configPath));
        File.WriteAllText(configPath, serializer.Serialize(config));
    }

    /// <summary>
    /// Reloads the configuration in memory with the one from the disk.
    /// </summary>
    /// <typeparam name="T">The type of configuration</typeparam>
    /// <param name="moduleName">The name of the module</param>
    /// <exception cref="InvalidOperationException">Thrown when the module already has a configuration registered with a different type</exception>
    public void ReloadConfig<T>(string moduleName) where T : class, new()
    {
        string yamlPath = GetConfigPath(moduleName);
        
        if (configurationCache.TryGetValue(moduleName, out var cachedConfig))
        {
            // Configuration is cached

            if (cachedConfig is not T)
            {
                // Different type
                throw new InvalidOperationException($"Configuration for '{moduleName}' is already loaded with a different type. Expected '{typeof(T)}'");
            }
        }

        T config;

        if (!File.Exists(yamlPath))
        {
            // Save default config to yaml
            config = new T();
            FileUtils.EnsureDirectoryExists(Path.GetDirectoryName(yamlPath));
            File.WriteAllText(yamlPath, serializer.Serialize(config));
        }
        else
        {
            string contents = File.ReadAllText(yamlPath);
            // Deserialize file contents into config variable
            // TODO: Deserialize<T> can return null, this must be handled
            try
            {
                config = deserializer.Deserialize<T>(contents);
            }
            catch (YamlException ex)
            {
                throw new ConfigurationException(moduleName, $"Failed to deserialize {yamlPath}, please check your configuration", ex);
            }
        }

        configurationCache.Remove(moduleName);
        configurationCache.Add(moduleName, config);
    }
}