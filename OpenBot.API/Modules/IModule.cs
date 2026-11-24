using OpenBot.API.Enums;

namespace OpenBot.API.Modules;

public interface IModule
{
    string Name { get; }
    ILogger Logger { get; }
    ModuleContext Context { get; }
    ModuleStatus Status  { get; }
}