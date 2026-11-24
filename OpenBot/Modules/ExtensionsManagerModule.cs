using System.Runtime.Loader;
using Newtonsoft.Json;
using OpenBot.API;
using OpenBot.API.Enums;
using OpenBot.API.Extensions;
using OpenBot.API.Modules;
using OpenBot.Utils;

namespace OpenBot.Modules;

internal sealed class ExtensionsManagerModule : Module, IExtensionsManagerModule
{
    private Dictionary<Type, Extension> extensions = [];
    internal ExtensionsManagerModule(ModuleContext context) : base(context)
    {
    }


    private readonly string ExtensionsDirectory = Path.Combine(API.OpenBot.GetRootDirectory(), "Extensions");

    internal async Task Register<T>(T extension, ExtensionMetadata metadata) where T : Extension
    {
        // Name length validation
        if (extension.Name.Length > 15)
        {
            throw new InvalidOperationException("Extension names cannot be longer than 15 characters!");
        }

        // Name taken validation
        if (extensions.ContainsKey(extension.GetType()) || extensions.ContainsValue(extension))
        {
            this.Logger.Warn($"Extension {extension.Name} is already registered, skipping...");
            return;
        }

        // API version validation
        if (metadata.apiVersion != APIDetails.ApiVersion.ToString())
        {
            this.Logger.Warn($"Extension {extension.Name} is compiled for a different API version than OpenBot, skipping...");
            return;
        }

        // Now register the extension
        extensions.Add(extension.GetType(), extension);
        //this.Logger.Info($"Registered extension {extension.Name}");
        try
        {
            await extension.RegisterAsync();
        }
        catch (Exception ex)
        {
            this.Logger.Error($"Failed to register extension {extension.Name}: {ex}");
        }
    }

    /// <summary>
    /// Starts all registered extensions
    /// </summary>
    /// <returns></returns>
    internal async Task StartAllExtensionsAsync()
    {
        foreach (Extension extension in extensions.Values)
        {
            if (extension.Status == ExtensionStatus.Started)
            {
                this.Logger.Warn($"Extension {extension.Name} is already started, skipping start...");
                continue;
            }
            try
            {
                await extension.StartAsync();
                this.Logger.Info($"Started extension {extension.Name}");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Failed to start extension {extension.Name}: {ex}");
            }
        }
    }

    /// <summary>
    /// Stops all regietered extensions
    /// </summary>
    /// <returns></returns>
    internal async Task StopAllExtensionsAsync()
    {
        foreach (Extension extension in extensions.Values.Reverse())
        {
            if (extension.Status != ExtensionStatus.Started)
            {
                this.Logger.Warn($"Extension {extension.Name} is not started, skipping stop...");
                continue;
            }
            try
            {
                await extension.StopAsync();
                this.Logger.Info($"Stopped extension {extension.Name}");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Failed to stop extension {extension.Name}: {ex}");
            }
        }
    }

    internal ExtensionMetadata[] DiscoverExtensions()
    {
        this.Logger.Debug($"Discovering extensions at {ExtensionsDirectory}...");

        FileUtils.EnsureDirectoryExists(ExtensionsDirectory);

        string[] directories = Directory.GetDirectories(
            ExtensionsDirectory,
            "*",
            SearchOption.TopDirectoryOnly
        );

        List<string> metaFiles = [];
        List<string> dllFiles = [];
        List<ExtensionMetadata> metas = [];

        foreach (string directory in directories)
        {
            try
            {
                // Here, directory = "Extensions/possibleExtensionName"
                string extensionName = Path.GetFileName(directory);

                string requiredExtensionMetadataPath = Path.Combine(directory, "extension.json");
                ExtensionMetadata? meta = GetExtensionMetadata(extensionName);

                if (meta is null)
                {
                    this.Logger.Warn($"Could not get metadata for extension {extensionName}, skipping...");
                    continue;
                }

                string requiredExtensionDllPath = Path.Combine(directory, meta.file ?? $"{extensionName}.dll");
                if (File.Exists(requiredExtensionDllPath))
                {
                    metas.Add(meta);
                    metaFiles.Add(requiredExtensionMetadataPath);
                    dllFiles.Add(requiredExtensionDllPath);
                }
                else
                {
                    this.Logger.Warn($"Skipping loading extension {extensionName}: The extension directory does not contain {meta.file}");
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn($"Error loading extension: {ex}");
            }
        }

        return metas.ToArray();
    }

    internal Type[] LoadExtensionTypes(string dllPath)
    {
        // TODO: AssemblyLoadContext.Default is temporary!!
        System.Reflection.Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
        Type[] unfilteredTypes = assembly.GetTypes();

        IEnumerable<Type> filteredTypes = 
            from Type type in unfilteredTypes
            where typeof(Extension).IsAssignableFrom(type)
                && !type.IsAbstract
                && type != typeof(Extension)
            select type;

        return filteredTypes.ToArray();
    }









    // == API Exposed Methods ==
    public ExtensionMetadata? GetExtensionMetadata(Extension extension)
    {
        return GetExtensionMetadata(extension.Name);
    }

    public ExtensionMetadata? GetExtensionMetadata(string extensionName)
    {
        try
        {
            string json = File.ReadAllText(Path.Combine(API.OpenBot.GetRootDirectory(), "Extensions", extensionName, "extension.json"));

            ExtensionMetadata? extensionMetadata = JsonConvert.DeserializeObject<ExtensionMetadata>(json);

            return extensionMetadata;
        }
        catch (Exception ex)
        {
            this.Logger.Warn($"Error getting extension metadata for {extensionName}: {ex}");
            throw new InvalidOperationException($"Failed to get extension metadata for {extensionName}");
        }
    }

    public IReadOnlyCollection<Extension> GetExtensions()
    {
        return extensions.Values;
    }

    public int GetExtensionsCount()
    {
        return extensions.Count;
    }

    public Extension? GetOrNull(string name)
    {
        Extension[] matchedExtensions = 
            (
                from Extension extension in extensions.Values
                where extension.Name == name
                select extension
            ).ToArray();

        if (matchedExtensions.Length == 1)
        {
            return matchedExtensions[0];
        }
        else if (matchedExtensions.Length > 1)
        {
            this.Logger.Error($"Multiple extensions are registered with a name of '{name}'");
            throw new InvalidOperationException($"There are multiple registered extensions of name '{name}'");
        }

        return null;
    }

    public T? GetOrNull<T>() where T : Extension
    {
        return extensions.TryGetValue(typeof(T), out var extension)
            ? extension as T
            : null;
    }

    public Extension GetOrThrow(string name)
    {
        Extension[] matchedExtensions = 
            (
                from Extension extension in extensions.Values
                where extension.Name == name
                select extension
            ).ToArray();

        if (matchedExtensions.Length == 1)
        {
            return matchedExtensions[0];
        }
        else if (matchedExtensions.Length > 1)
        {
            this.Logger.Error($"Multiple extensions are registered with a name of '{name}'");
            throw new InvalidOperationException($"There are multiple registered extensions of name '{name}'");
        }

        throw new InvalidOperationException($"Failed to retrieve extension {name}.");
    }

    public T GetOrThrow<T>() where T : Extension
    {
        extensions.TryGetValue(typeof(T), out var extension);
        return extension as T ?? throw new InvalidOperationException("Failed to retrieve extension.");
    }

    public bool IsRegistered<T>() where T : Extension
    {
        return extensions.ContainsKey(typeof(T));
    }

    public bool IsStarted<T>() where T : Extension
    {
        return extensions.ContainsKey(typeof(T)) && extensions[typeof(T)].Status == ExtensionStatus.Started;
    }
}