using OpenBot.API.Events;
using OpenBot.API.Modules;

namespace OpenBot.Events;

internal sealed class OpenBotEvents : IOpenBotEvents
{
    public event EventHandler? BotStarted;
    public event EventHandler? BotStopping;
    public event EventHandler<ModuleStartedEventArgs>? ModuleStarted;
    public event EventHandler<ExtensionStartedEventArgs>? ExtensionStarted;

    internal void RaiseBotStarted()
    {
        BotStarted?.Invoke(this, EventArgs.Empty);
    }

    internal void RaiseBotStopping()
    {
        BotStopping?.Invoke(this, EventArgs.Empty);
    }

    internal void RaiseModuleStarted(Module module)
    {
        ModuleStartedEventArgs args = new(module);
        ModuleStarted?.Invoke(this, args);
    }

    internal void RaiseExtensionStarted(Extension extension)
    {
        ExtensionStartedEventArgs args = new(extension);
        ExtensionStarted?.Invoke(this, args);
    }
}