using OpenBot.API;
using OpenBot.Modules;

var moduleManager = new ModuleManager();

var coreContext = new ModuleContext(
    "Core",
    LoggingService.GetLogger("Core"),
    moduleManager,
    null
);

Core core = new(moduleManager, coreContext);

await core.RunAsync();
