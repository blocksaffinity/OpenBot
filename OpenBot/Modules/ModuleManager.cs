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

        try
        {
            await module.RegisterAsync();
        }
        catch (Exception ex)
        {
            logger.Error($"Failed to register module {module.Name}: {ex}");
            throw;
        }

        modules.Add(module.GetType(), module);
        //this.logger.Info($"Registered module {module.Name}");

        this.logger.Debug($"Registered module: {module.Name}");
        this.logger.Debug($"Public module: {module is not PrivateModule}");
        this.logger.Debug($"Module type: {module.GetType().FullName}");
        

        if (module is not PrivateModule)
        {
            publicModules.Add(module.GetType(), module);

            // WTF even is this?!
            foreach (var type in module.GetType().GetInterfaces())
            {
                if (typeof(IModule).IsAssignableFrom(type))
                {
                    publicModules[type] = module;
                }
            }
        }
    }

    /// <summary>
    /// Starts all registered modules<br/>
    /// Throws an exception of a module threw one
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
                throw;
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
                this.logger.Info($"Stopped module {module.Name}");
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


    // Do not expose    
    IModule? IModuleManager.GetOrNull_IncPrivate(string name)
    {
        IModule[] matchedModules = 
            (
                //                     V-- The only difference lol
                from IModule module in modules.Values
                where module.Name == name
                select module
            ).ToArray();

        if (matchedModules.Length == 1)
        {
            return matchedModules[0];
        }
        else if (matchedModules.Length > 1)
        {
            this.logger.Error($"Multiple modules are registered with a name of '{name}'");
            throw new InvalidOperationException($"There are multiple registered modules of name '{name}'");
        }

        return null;
    }






    // == API Exposed Methods ==

    public IReadOnlyCollection<IModule> GetModules()
    {
        return publicModules.Values.Distinct().ToArray();
    }

    public int GetModulesCount()
    {
        return publicModules.Count;
    }

    public IModule? GetOrNull(string name)
    {
        IModule[] matchedModules = 
            (
                from IModule module in publicModules.Values
                where module.Name == name
                select module
            ).ToArray();

        if (matchedModules.Length == 1)
        {
            return matchedModules[0];
        }
        else if (matchedModules.Length > 1)
        {
            this.logger.Error($"Multiple modules are registered with a name of '{name}'");
            throw new InvalidOperationException($"There are multiple registered modules of name '{name}'");
        }

        return null;
    }

    public T? GetOrNull<T>() where T : class, IModule
    {
        return publicModules.TryGetValue(typeof(T), out var module)
            ? module as T
            : null;
    }

    public IModule GetOrThrow(string name)
    {
        IModule[] matchedModules = 
            (
                from IModule module in publicModules.Values
                where module.Name == name
                select module
            ).ToArray();

        if (matchedModules.Length == 1)
        {
            return matchedModules[0];
        }
        else if (matchedModules.Length > 1)
        {
            this.logger.Error($"Multiple modules are registered with a name of '{name}'");
            throw new InvalidOperationException($"There are multiple registered modules of name '{name}'");
        }

        throw new InvalidOperationException($"Failed to retrieve module {name}.");
    }

    public T GetOrThrow<T>() where T : class, IModule
    {
        publicModules.TryGetValue(typeof(T), out var module);
        return module as T ?? throw new InvalidOperationException("Failed to retrieve module.");
    }

    public bool IsRegistered<T>() where T : class, IModule
    {
        return publicModules.ContainsKey(typeof(T));
    }

    public bool IsStarted<T>() where T : class, IModule
    {
        return publicModules.ContainsKey(typeof(T))
                &&
                publicModules[typeof(T)].Status == ModuleStatus.Started;
    }
}