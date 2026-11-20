namespace OpenBot.API.Modules;

public interface IModuleManager
{
    IReadOnlyCollection<IModule> GetModules();

    T? Get<T>() where T : class, IModule;
}