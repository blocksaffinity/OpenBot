namespace OpenBot.API.Modules;

public interface IModuleProvider
{
    IReadOnlyCollection<IModule> GetModules();

    T? Get<T>() where T : class, IModule;
}