using System.Runtime.Loader;
using Newtonsoft.Json;
using OpenBot.API;
using OpenBot.API.Enums;
using OpenBot.API.Extensions;
using OpenBot.API.Modules;
using OpenBot.Utils;

namespace OpenBot.Modules;

internal sealed class ExtensionsManagerModule : Module, IExtensionManagerModule
{
    private Dictionary<Type, Extension> extensions = [];
    internal ExtensionsManagerModule(ModuleContext context) : base(context)
    {
    }

    public ExtensionMetadata? GetExtensionMetadata(Extension extension)
    {
        return GetExtensionMetadata(extension.Name);
    }

    public ExtensionMetadata? GetExtensionMetadata(string extensionName)
    {
        try
        {
            // TODO: Definitely jank, urgent help needed
            string json = File.ReadAllText(Path.Combine("..", "..", "..", "..", "Extensions", extensionName, "extension.json"));

            ExtensionMetadata? extensionMetadata = JsonConvert.DeserializeObject<ExtensionMetadata>(json);

            return extensionMetadata;
        }
        catch (Exception ex)
        {
            // Probably jank, possibly not
            this.Logger.Warn($"Error getting extension metadata for {extensionName}: {ex}");
            return null;
        }
    }

    private readonly string ExtensionsDirectory =
    //TODO: jank, MUST fix .. x4 it doesn't even work in my main terminal
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..", "Extensions"
            )
        );

    internal async Task Register<T>(T extension) where T : Extension
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
        
        ExtensionMetadata? meta = GetExtensionMetadata(extension);
        // Metadata validation
        if (meta is null)
        {
            this.Logger.Warn($"Extension {extension.Name} does not appear to contain metadata at extension.json, skipping...");
            return;
        }

        // API version validation
        if (meta.apiVersion != APIDetails.APIVersion)
        {
            this.Logger.Warn($"Extension {extension.Name} is compiled for a different API version than OpenBot, skipping...");
            return;
        }

        // Now register the extension
        extensions.Add(extension.GetType(), extension);
        this.Logger.Info($"Registered extension {extension.Name}");
        await extension.RegisterAsync();
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
                this.Logger.Info($"Stoped extension {extension.Name}");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Failed to stop extension {extension.Name}: {ex}");
            }
        }
    }

    public T? Get<T>() where T : Extension
    {
        return extensions.TryGetValue(typeof(T), out var module)
            ? module as T
            : null;
    }

    public Extension? Get(string name)
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

    public IReadOnlyCollection<Extension> GetExtensions()
    {
        return extensions.Values;
    }

    public int GetExtensionsCount()
    {
        return extensions.Count;
    }

    internal string[] Discover()
    {
        this.Logger.Debug($"Discovering extensions at {ExtensionsDirectory}...");

        FileUtils.EnsureDirectoryExists(ExtensionsDirectory);

        string[] directories = Directory.GetDirectories(
            ExtensionsDirectory,
            "*",
            SearchOption.TopDirectoryOnly
        );

        List<string> files = [];

        foreach (string directory in directories)
        {
            // Here, directory = "Extensions/possibleExtensionName"
            string extensionName = Path.GetFileName(directory);

            string requriedExtensionDllPath =
                Path.Combine(directory, $"{extensionName}.dll");

            if (File.Exists(requriedExtensionDllPath))
            {
                this.Logger.Debug($"Discovered {extensionName}.dll");
                files.Add(requriedExtensionDllPath);
            }
            else
            {
                this.Logger.Warn($"Skipping loading extension {extensionName}: The extension directory does not contain {extensionName}.dll");
            }
        }

        return files.ToArray();
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
}
