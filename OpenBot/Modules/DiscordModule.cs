using Discord;
using Discord.WebSocket;
using OpenBot.API;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal sealed class DiscordModule : Module, IDiscordModule
{
    public DiscordSocketClient Client { get; }
    private readonly ConfigurationClasses.ModuleConfig.DiscordConfig config;

    internal DiscordModule(ModuleContext context, AuthenticationModule authentication) : base(context)
    {
        config = this.GetConfig<ConfigurationClasses.ModuleConfig.DiscordConfig>();

        // This thing makes sure there's no invalid intents by doing some binary black magic
        if (((ulong)config.GatewayIntents & ~(ulong)GatewayIntents.All) != 0)
        {
            throw new InvalidOperationException("Discord module configuration contains invalid GatewayIntents. This is a configuration issue: check /ModuleConfig/Discord/config.yml!");
        }

        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = config.GatewayIntents
        });

        Client.Log += LogAsync;

        // Client ready event
        Client.Ready += () =>
        {
            Logger.Info($"Connected as {Client.CurrentUser}");
            return Task.CompletedTask;
        };

        this.authentication = authentication;
    }

    private readonly AuthenticationModule authentication;

    protected override async Task OnStartAsync()
    {
        this.Logger.Debug("Getting token...");

        var token = authentication.GetToken();

        this.Logger.Debug("Connecting to bot...");

        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();
    }

    protected override async Task OnStopAsync()
    {
        this.Logger.Info("Disconnecting client...");
        await Client.StopAsync();
        await Client.LogoutAsync();
    }

    private Task LogAsync(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Debug or LogSeverity.Verbose:
                this.Logger.Debug(msg.Message);
                break;
            
            case LogSeverity.Info:
                this.Logger.Info(msg.Message);
                break;

            case LogSeverity.Warning:
                this.Logger.Warn(msg.Message);
                break;
            
            case LogSeverity.Error:
                this.Logger.Error(msg.Message);
                break;
            
            case LogSeverity.Critical:
                this.Logger.Fatal(msg.Message);
                break;
        }
        return Task.CompletedTask;
    }
}
