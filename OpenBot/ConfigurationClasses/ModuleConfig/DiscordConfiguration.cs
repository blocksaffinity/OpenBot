namespace OpenBot.ConfigurationClasses.ModuleConfig;

internal sealed class DiscordConfig
{
    public Discord.GatewayIntents GatewayIntents { get; set; } = 
        Discord.GatewayIntents.Guilds |
        Discord.GatewayIntents.GuildMessages |
        Discord.GatewayIntents.MessageContent;
}