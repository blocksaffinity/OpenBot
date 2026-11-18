using System.Numerics;
using OpenBot.API.Enums;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal sealed class ModuleManager : IModuleProvider
{
    private readonly Dictionary<Type, Module> modules = [];
    private readonly Dictionary<Type, IModule> publicModules = [];
    private readonly Logger logger;

    internal ModuleManager()
    {
        logger = LoggingService.GetLogger("ModuleManager");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the desired module</typeparam>
    /// <param name="module">The module to register</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void Register<T>(T module) where T : Module
    {
        if (module.Name.Length > 15)
        {
            throw new InvalidOperationException("Module/extension names cannot be longer than 15 characters!");
        }

        if (modules.ContainsValue(module))
        {
            this.logger.Warn($"{module.Name} is already registered, skipping...");
            return;
        }

        modules.Add(typeof(T), module);
        

        foreach (var type in module.GetType().GetInterfaces())
        {
            if (typeof(IModule).IsAssignableFrom(type))
            {
                publicModules[type] = module;
                this.logger.Info($"Registered module {module.Name}");
            }
        }
    }

    /// <summary>
    /// Starts all registered modules
    /// </summary>
    internal async Task StartAllModulesAsync()
    {
        foreach (IModule module in modules.Values)
        {
            if (((IModuleLifecycle)module).Status == ModuleStatus.Started)
            {
                this.logger.Warn($"{module.Name} is already started, skipping start...");
                continue;
            }
            await ((IModuleLifecycle)module).StartAsync();
            this.logger.Info($"Started module {module.Name}");
        }
    }

    internal async Task StopAllModulesAsync()
    {
        foreach (IModule module in modules.Values.Reverse())
        {
            if (((IModuleLifecycle)module).Status != ModuleStatus.Started)
            {
                this.logger.Warn($"{module.Name} is not started, skipping stop...");
                continue;
            }
            await ((IModuleLifecycle)module).StopAsync();
            this.logger.Info($"Stoped module {module.Name}");
        }
    }

    internal T GetInternal<T>() where T : Module
    {
        return (T)modules[typeof(T)];
    }

    public IReadOnlyCollection<IModule> GetModules()
    {
        return publicModules.Values.Distinct().ToArray();
    }

    public T? Get<T>() where T : class, IModule
    {
        return publicModules.TryGetValue(typeof(T), out var module)
            ? module as T
            : null;
    }
}