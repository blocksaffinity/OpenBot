using System.Reflection.PortableExecutable;
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

    internal async Task StartAsync()
    {
        this.Logger.Debug("Starting bot...");

        this.Logger.Debug("Registering modules...");

        // #1 Configuration module
        var configurationContext = new ModuleContext(
            "Configuration",
            LoggingService.GetLogger("Configuration"),
            moduleManager,
            null
        );
        var configuration = new ConfigurationModule(configurationContext);
        moduleManager.Register(configuration);

        // #2 Authentication module
        var authenticationContext = new ModuleContext(
            "Authentication",
            LoggingService.GetLogger("Authentication"),
            moduleManager,
            configuration
        );
        var authentication = new AuthenticationModule(authenticationContext);
        moduleManager.Register(authentication);

        // FIXME: This registers twice??
        // #3 Discord module
        var discordContext = new ModuleContext(
            "Discord",
            LoggingService.GetLogger("Discord"),
            moduleManager,
            configuration
        );
        var discord = new DiscordModule(discordContext, authentication);
        moduleManager.Register(discord);

        await moduleManager.StartAllModulesAsync();

        // Connect to bot
        await discord.StartAsync();

        // If there's no extensions loaded
        // TODO: implement this statement once extension manager is implemented
        this.Logger.Hint("No extensions are loaded! Make sure to add some to give your bot some functionality!");

        this.Logger.Debug("Waiting for eternity...");

        await Task.Delay(Timeout.Infinite);
    }
}