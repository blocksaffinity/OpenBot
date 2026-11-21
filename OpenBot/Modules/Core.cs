using OpenBot.API;
using OpenBot.API.Extensions;
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

        this.Logger.Hint($"OpenBot directory: {API.OpenBot.GetRootDirectory()}");

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

        ExtensionMetadata[] extensionMetas = extensions.DiscoverExtensions();

        // Assembly loading stuff
        foreach (ExtensionMetadata extensionMeta in extensionMetas)
        {
            if (extensionMeta is null)
            {
                // A log message has already been written
                // Just skip this extension
                continue;
            }

            string extensionName = extensionMeta.name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(extensionName))
            {
                this.Logger.Warn($"Skipped loading (unknown extension): extension.json does not contain 'name'.");
                continue;
            }
            try
            {
                string dllFile = extensionMeta.file ?? $"{extensionName}.dll";
                string dllPath = Path.Combine(API.OpenBot.GetRootDirectory(), "Extensions", extensionName, dllFile);
                Type[] types = extensions.LoadExtensionTypes(dllPath: dllPath);
                Extension? extension = null;
                Logger logger = LoggingService.GetLogger(extensionName);


                foreach (Type type in types)
                {
                    ExtensionContext extensionContext = new(
                        name                 : extensionName,
                        logger               : logger,
                        modules              : moduleManager,
                        configurationProvider: configuration,
                        meta                 : extensionMeta
                    );
                    object? instance = Activator.CreateInstance(type, extensionContext);

                    if (instance is Extension validExtension)
                    {
                        extension = validExtension;
                    }
                }
                if (extension is not null)
                {
                    await extensions.Register(extension, extensionMeta);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to load extension {extensionName}: {ex}");
            }
        }

        await extensions.StartAllExtensionsAsync();

        // If there's no extensions loaded
        if (extensions.GetExtensionsCount() < 1)
        {
            this.Logger.Hint("No extensions are loaded! Make sure to add some to give your bot some functionality!");
        }


        if (APIDetails.ApiPreReleaseVersion)
        {
            this.Logger.Warn("This version of the OpenBot API is a beta version. Expect bugs and instability.");
        }


        this.Logger.Debug("Waiting for eternity...");

        // TODO: Implement a graceful shutdown
        await Task.Delay(Timeout.Infinite);
    }
}