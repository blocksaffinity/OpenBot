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

        await moduleManager.StartAllModulesAsync();

        // If there's no extensions loaded
        // TODO: implement this statement once extension manager is implemented
        this.Logger.Hint("No extensions are loaded! Make sure to add some to give your bot some functionality!");

        this.Logger.Debug("Waiting for eternity...");

        // TODO: Implement a graceful shutdown
        await Task.Delay(Timeout.Infinite);
    }
}