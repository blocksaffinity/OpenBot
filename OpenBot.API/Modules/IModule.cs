using OpenBot.API.Enums;

namespace OpenBot.API.Modules;

public interface IModule
{
    string Name { get; }
    ModuleStatus Status { get; }
}