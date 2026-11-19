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
            try
            {
                Type[] types = extensions.LoadExtensionTypes(extensionPath);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Failed to load extension {Path.GetFileName(extensionPath)}: {ex}");
            }
        }

        // If there's no extensions loaded
        // TODO: implement this statement once extension manager is implemented
        this.Logger.Hint("No extensions are loaded! Make sure to add some to give your bot some functionality!");

        this.Logger.Debug("Waiting for eternity...");

        // TODO: Implement a graceful shutdown
        await Task.Delay(Timeout.Infinite);
    }
}