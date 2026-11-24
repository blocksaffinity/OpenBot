using OpenBot.API;
using OpenBot.Modules;

var moduleManager = new ModuleManager();

var coreContext = new ModuleContext(
    "Core",
    LoggingService.GetLogger("Core"),
    moduleManager
);

Core core = new(moduleManager, coreContext);

await core.RunAsync();
