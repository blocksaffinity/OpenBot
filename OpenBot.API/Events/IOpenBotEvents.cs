namespace OpenBot.API.Events;

public interface IOpenBotEvents
{
    /// <summary>
    /// Raised once all modules and extensions are fully started.
    /// </summary>
    event EventHandler? BotStarted;
    
    /// <summary>
    /// Raised once the bot begins to shut down.
    /// </summary>
    event EventHandler? BotStopping;

    /// <summary>
    /// Raised upon the successful start of a module
    /// </summary>
    public event EventHandler<ModuleStartedEventArgs>? ModuleStarted;

    /// <summary>
    /// Raised upon the successful start of an extension
    /// </summary>
    public event EventHandler<ExtensionStartedEventArgs>? ExtensionStarted;
}