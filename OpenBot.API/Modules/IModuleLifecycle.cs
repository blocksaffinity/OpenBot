using OpenBot.API.Enums;

namespace OpenBot.API.Modules;

public interface IModuleLifecycle
{
    ModuleStatus Status { get; }
    Task StartAsync();
    Task StopAsync();
}