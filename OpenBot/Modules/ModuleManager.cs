using OpenBot.API.Enums;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal sealed class ModuleManager : IModuleManager
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
    internal async Task Register<T>(T module) where T : Module
    {
        if (module.Name.Length > 15)
        {
            throw new InvalidOperationException("Module names cannot be longer than 15 characters!");
        }

        if (modules.ContainsKey(module.GetType()) || modules.ContainsValue(module))
        {
            this.logger.Warn($"Module{module.Name} is already registered, skipping...");
            return;
        }

        modules.Add(module.GetType(), module);
        //this.logger.Info($"Registered module {module.Name}");
        await module.RegisterAsync();

        foreach (var type in module.GetType().GetInterfaces())
        {
            if (typeof(IModule).IsAssignableFrom(type))
            {
                publicModules[type] = module;
            }
        }
    }

    /// <summary>
    /// Starts all registered modules
    /// </summary>
    internal async Task StartAllModulesAsync()
    {
        foreach (Module module in modules.Values)
        {
            if (module.Status == ModuleStatus.Started)
            {
                this.logger.Warn($"Module {module.Name} is already started, skipping start...");
                continue;
            }
            try
            {
                await module.StartAsync();
                this.logger.Info($"Started module {module.Name}");
            }
            catch (Exception ex)
            {
                this.logger.Error($"Failed to start module {module.Name}: {ex}");
            }
        }
    }

    /// <summary>
    /// Stops all registered modules
    /// </summary>
    internal async Task StopAllModulesAsync()
    {
        foreach (Module module in modules.Values.Reverse())
        {
            if (module.Status != ModuleStatus.Started)
            {
                this.logger.Warn($"Module {module.Name} is not started, skipping stop...");
                continue;
            }
            try
            {
                await module.StopAsync();
                this.logger.Info($"Stoped module {module.Name}");
            }
            catch (Exception ex)
            {
                this.logger.Error($"Failed to stop module {module.Name}: {ex}");
            }
        }
    }

    internal T GetInternal<T>() where T : Module
    {
        return (T)modules[typeof(T)];
    }


    /// <summary>
    /// Retrieves the list of public modules
    /// </summary>
    /// <returns>List of public registered modules</returns>
    public IReadOnlyCollection<IModule> GetModules()
    {
        return publicModules.Values.Distinct().ToArray();
    }

    /// <summary>
    /// Retrieves a registered module of the provided type, or null if one is not registered
    /// </summary>
    /// <typeparam name="T">The class, e.g. ConfigurationModule, of the target module (must inherit from IModule)</typeparam>
    /// <returns>The requested module</returns>
    public T? Get<T>() where T : class, IModule
    {
        return publicModules.TryGetValue(typeof(T), out var module)
            ? module as T
            : null;
    }
}