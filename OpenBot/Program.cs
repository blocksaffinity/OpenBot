using OpenBot.API;
using OpenBot.Modules;

Logger logger = LoggingService.GetLogger("Program");

var moduleManager = new ModuleManager();

var coreContext = new ModuleContext(
    "Core",
    LoggingService.GetLogger("Core"),
    moduleManager
);

Core core = new(moduleManager, coreContext);

try
{
    await core.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal($"An uncaught {ex.GetType()} occurred: {ex.Message}\n{ex.StackTrace}");
}

logger.Info("Goodbye, thanks for using OpenBot!");
