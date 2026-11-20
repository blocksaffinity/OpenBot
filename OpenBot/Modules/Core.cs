using OpenBot.API;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal class Core : Module
{
    private readonly ModuleManager moduleManager;

    internal Core(ModuleManager moduleManager, ModuleContext ctx) : base(ctx)
    {
        this.moduleManager = moduleManager;
    }

    internal async Task RunAsync()
    {
        this.Logger.Info("Hello and welcome to OpenBot!");

        this.Logger.Debug("Registering modules...");

        // #1 Configuration module
        var configurationContext = new ModuleContext(
            "Configuration",
            LoggingService.GetLogger("Configuration"),
            moduleManager,
            null
        );
        var configuration = new ConfigurationModule(configurationContext);
        await moduleManager.Register(configuration);

        // #2 Authentication module
        var authenticationContext = new ModuleContext(
            "Authentication",
            LoggingService.GetLogger("Authentication"),
            moduleManager,
            configuration
        );
        var authentication = new AuthenticationModule(authenticationContext);
        await moduleManager.Register(authentication);

        // #3 Discord module
        var discordContext = new ModuleContext(
            "Discord",
            LoggingService.GetLogger("Discord"),
            moduleManager,
            configuration
        );
        var discord = new DiscordModule(discordContext, authentication);
        await moduleManager.Register(discord);

        // #4 Extensions module
        var extensionsContext = new ModuleContext(
            "Extensions",
            LoggingService.GetLogger("Extensions"),
            moduleManager,
            configuration
        );
        var extensions = new ExtensionsManagerModule(extensionsContext);
        await moduleManager.Register(extensions);

        await moduleManager.StartAllModulesAsync();



        string[] extensionPaths = extensions.Discover();

        // Assembly loading stuff
        foreach (string extensionPath in extensionPaths)
        {
            string extensionName = Path.GetFileName(Path.GetDirectoryName(extensionPath)) ?? string.Empty;
            try
            {
                Type[] types = extensions.LoadExtensionTypes(extensionPath);
                Extension? extension = null;
                foreach (Type type in types)
                {
                    // (Parameter names are not accurate)
                    // ExtensionContext(string name, ILogger logger, IModuleManager modMan, IConfigurationModule conf)
                    ExtensionContext extensionContext = new(
                        extensionName,
                        LoggingService.GetLogger(extensionName),
                        moduleManager,
                        configuration
                    );
                    object? instance = Activator.CreateInstance(type, extensionContext);

                    if (instance is Extension validExtension)
                    {
                        extension = validExtension;
                    }
                }
                if (extension is not null)
                {
                    await extensions.Register(extension);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to load extension {extensionName}: {ex}");
            }
        }

        // If there's no extensions loaded
        if (extensions.GetExtensionsCount() < 1)
        {
            this.Logger.Hint("No extensions are loaded! Make sure to add some to give your bot some functionality!");
        }

        this.Logger.Debug("Waiting for eternity...");

        // TODO: Implement a graceful shutdown
        await Task.Delay(Timeout.Infinite);
    }
}