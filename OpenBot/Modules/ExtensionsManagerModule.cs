using System.Runtime.Loader;
using OpenBot.API;
using OpenBot.API.Modules;
using OpenBot.Utils;

namespace OpenBot.Modules;

internal sealed class ExtensionsManagerModule : Module
{
    internal ExtensionsManagerModule(ModuleContext context) : base(context)
    {
    }

    private readonly string ExtensionsDirectory =
    //TODO: jank, MUST fix .. x4 it doesn't even work in my main terminal
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..", "Extensions"
            )
        );

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
                this.Logger.Debug($"Discovered extension {extensionName}");
                files.Add(requriedExtensionDllPath);

                // TODO: Implement extension loading and then uncomment this line
                //this.Logger.Debug($"Loading extension {extensionName}...");
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
