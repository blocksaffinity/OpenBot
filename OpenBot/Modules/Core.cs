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

        
        // #1 Configuration module
        var configurationContext = new ModuleContext(
            "Configuration",
            LoggingService.GetLogger("Configuration"),
            moduleManager
        );
        var configuration = new ConfigurationModule(configurationContext);


        // #2 Authentication module
        var authenticationContext = new ModuleContext(
            "Authentication",
            LoggingService.GetLogger("Authentication"),
            moduleManager
        );
        var authentication = new AuthenticationModule(authenticationContext);


        // #3 Discord module
        var discordContext = new ModuleContext(
            "Discord",
            LoggingService.GetLogger("Discord"),
            moduleManager
        );
        var discord = new DiscordModule(discordContext, authentication);


        // #4 Extensions module
        var extensionsContext = new ModuleContext(
            "Extensions",
            LoggingService.GetLogger("Extensions"),
            moduleManager
        );
        var extensions = new ExtensionsManagerModule(extensionsContext);


        // #5 ConsoleCommandHandler module
        var cchContext = new ModuleContext(
            "CCommandHandler",
            LoggingService.GetLogger("CCommandHandler"),
            moduleManager
        );
        var cch = new ConsoleCommandHandlerModule(cchContext);

        try
        {
            await moduleManager.Register(configuration);
            await moduleManager.Register(authentication);
            await moduleManager.Register(discord);
            await moduleManager.Register(extensions);
            await moduleManager.Register(cch);
        }
        catch
        {
            // Don't include the exception itself, already been logged in moduleManager.Register()
            this.Logger.Fatal("A module failed to register, startup aborted.");
            return;
        }

        try
        {
            // Modules are required, so if one fails to start we're gonna stop the entire bot from starting
            await moduleManager.StartAllModulesAsync();   
        }
        catch
        {
            // Don't include the exception itself, already been logged in moduleManager.StartAllModulesAsync()
            this.Logger.Fatal("A module failed to start, startup aborted.");
            return;
        }

        // This doesn't throw
        ExtensionMetadata[] extensionMetas = extensions.DiscoverExtensions();

        // Assembly loading stuff
        foreach (ExtensionMetadata extensionMeta in extensionMetas)
        {
            if (extensionMeta is null)
            {
                // A log message has already been written [Apparently..?]
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
                        name    : extensionName,
                        logger  : logger,
                        modules : moduleManager,
                        meta    : extensionMeta
                    );
                    try
                    {
                        object? instance = Activator.CreateInstance(type, extensionContext);

                        if (instance is Extension validExtension)
                        {
                            extension = validExtension;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Exceptions can be thrown whilst constructing extensions or modules
                        logger.Error($"Failed to construct extension {extensionName}, it will not be loaded. {ex}");
                        continue;
                    }
                    
                }
                if (extension is not null)
                {
                    try
                    {
                        await extensions.Register(extension, extensionMeta);
                    }
                    catch
                    {
                        // Don't include the exception itself, already been logged in extensions.Register()
                        this.Logger.Error($"Failed to register extension {extensionName}, it will not be available.");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn($"Failed to load extension {extensionName}: {ex}");
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


        // TODO: Implement a graceful shutdown
        // Note: running `stop` is graceful
        //await Task.Delay(Timeout.Infinite);

        await cch.CommandHandlerLoop(extMan: extensions, modMan: moduleManager);
    }
}