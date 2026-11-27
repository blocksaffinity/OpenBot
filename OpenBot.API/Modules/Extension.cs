using OpenBot.API.Enums;
using OpenBot.API.Extensions;

namespace OpenBot.API.Modules;

/// <summary>
/// Base class for an OpenBot extension
/// </summary>
public abstract class Extension
{
    /// <summary>
    /// The name of the extension
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The extension's ILogger instance
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// The extension's ExtensionContext instance
    /// </summary>
    public ExtensionContext Context { get; }

    /// <summary>
    /// The ExtensionStatus of the extension
    /// </summary>
    public ExtensionStatus Status  { get; private set; } = ExtensionStatus.Unregistered;

    public ExtensionMetadata Metadata { get; }

    protected Extension(ExtensionContext context)
    {
        Context = context;
        Name = context.Name;
        Logger = context.Logger;
        Metadata = context.Metadata;
    }

    internal async Task RegisterAsync()
    {
        await OnRegisterAsync();
        Status = ExtensionStatus.Registered;
    }
    internal async Task StartAsync()
    {
        Context.DiscordModule = Context.Modules.GetOrThrow<IDiscordModule>();
        Context.ConfigurationModule = Context.Modules.GetOrThrow<IConfigurationModule>();
        await OnStartAsync();
        Status = ExtensionStatus.Started;
    }

    internal async Task StopAsync()
    {
        await OnStopAsync();
        Status = ExtensionStatus.Registered;
    }

    /// <summary>
    /// Called upon the registration of the extension
    /// </summary>
    protected virtual Task OnRegisterAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called upon the start of the extension
    /// </summary>
    protected virtual Task OnStartAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called upon the shutdown/stop of the extension
    /// </summary>
    protected virtual Task OnStopAsync()
    {
        return Task.CompletedTask;
    }

    protected T GetConfig<T>() where T : class, new()
    {
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }

        return Context.ConfigurationModule.GetConfig<T>(Name);
    }

    protected void SaveConfig<T>(T config) where T : class
    {
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }

        Context.ConfigurationModule.SaveConfig(Name, config);
    }

    protected void ReloadConfig<T>() where T : class, new()
    {
        if (Context.ConfigurationModule is null)
        {
            throw new InvalidOperationException("Configuration is not available.");
        }
        
        Context.ConfigurationModule.ReloadConfig<T>(Name);
    }
}