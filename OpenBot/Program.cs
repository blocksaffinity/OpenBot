using OpenBot.API;
using OpenBot.Events;
using OpenBot.Modules;

Logger logger = LoggingService.GetLogger("Program");
OpenBotEvents events = new();
ModuleManager moduleManager = new(events);

var coreContext = new ModuleContext(
    "Core",
    LoggingService.GetLogger("Core"),
    moduleManager,
    events
);

Core core = new(moduleManager, coreContext, events);

try
{
    await core.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal($"An uncaught {ex.GetType()} occurred: {ex.Message}\n{ex.StackTrace}");
}

logger.Info("Goodbye, thanks for using OpenBot!");
