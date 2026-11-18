namespace OpenBot.API.Modules;

public interface IConfigurationProvider
{
    T GetConfig<T>(string moduleName) where T : class, new();

    void SaveConfig<T>(string moduleName, T config) where T : class;

    void ReloadConfig<T>(string moduleName) where T : class, new();
}