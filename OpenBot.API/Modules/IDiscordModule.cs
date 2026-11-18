using Discord.WebSocket;

namespace OpenBot.API.Modules;

public interface IDiscordModule : IModule
{
    DiscordSocketClient Client { get; }
}
