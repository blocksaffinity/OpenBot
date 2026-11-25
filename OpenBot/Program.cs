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

await core.RunAsync();

logger.Info("Goodbye, thanks for using OpenBot!");
