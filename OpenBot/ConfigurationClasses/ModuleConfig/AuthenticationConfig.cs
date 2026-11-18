namespace OpenBot.ConfigurationClasses.ModuleConfig;

internal sealed class AuthenticationConfig
{
    // TODO: This is *VERY* insecure!!!! Malicious extensions can just grab this from YAML.
    public string BotToken { get; set; } = string.Empty;

}