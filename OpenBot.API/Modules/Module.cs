using OpenBot.API.Enums;

namespace OpenBot.API.Modules;

public abstract class Module : IModule, IModuleLifecycle
{
    public string Name { get; }
    public ILogger Logger { get; }
    public ModuleContext Context { get; }
    public ModuleStatus Status  { get; private set; } = ModuleStatus.Unregistered;


    protected Module(ModuleContext context)
    {
        Context = context;
        Name = context.Name;
        Logger = context.Logger;
    }

    Task IModuleLifecycle.StartAsync()
    {
        Status = ModuleStatus.Started;
        return OnStartAsync();
    }

    Task IModuleLifecycle.StopAsync()
    {
        Status = ModuleStatus.Registered;
        return OnStopAsync();
    }

    protected virtual Task OnStartAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnStopAsync()
    {
        return Task.CompletedTask;
    }

    protected T GetConfig<T>() where T : class, new()
    {
        if (Context.Configuration is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }

        return Context.Configuration.GetConfig<T>(Name);
    }

    protected void SaveConfig<T>(T config) where T : class
    {
        if (Context.Configuration is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }

        Context.Configuration.SaveConfig(Name, config);
    }

    protected void ReloadConfig<T>() where T : class, new()
    {
        if (Context.Configuration is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }
        
        Context.Configuration.ReloadConfig<T>(Name);
    }
}