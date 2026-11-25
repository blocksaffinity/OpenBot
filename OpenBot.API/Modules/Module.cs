using OpenBot.API.Enums;

namespace OpenBot.API.Modules;

public abstract class Module : IModule
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

    internal async Task RegisterAsync()
    {
        await OnRegisterAsync();
        Status = ModuleStatus.Registered;
    }
    internal async Task StartAsync()
    {
        Context.DiscordModule = Context.Modules.GetOrThrow<IDiscordModule>();
        Context.ConfigurationModule = Context.Modules.GetOrThrow<IConfigurationModule>();
        await OnStartAsync();
        Status = ModuleStatus.Started;
    }

    internal async Task StopAsync()
    {
        await OnStopAsync();
        Status = ModuleStatus.Registered;
    }

    protected virtual Task OnRegisterAsync()
    {
        return Task.CompletedTask;
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
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("ConfigurationModule is null.");
        }

        return Context.ConfigurationModule.GetConfig<T>(Name);
    }

    protected void SaveConfig<T>(T config) where T : class
    {
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("ConfigurationModule is null.");
        }

        Context.ConfigurationModule.SaveConfig(Name, config);
    }

    protected void ReloadConfig<T>() where T : class, new()
    {
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("ConfigurationModule is null.");
        }
        
        Context.ConfigurationModule.ReloadConfig<T>(Name);
    }
}