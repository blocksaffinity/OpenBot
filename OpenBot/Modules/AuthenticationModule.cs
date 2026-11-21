using OpenBot.API;
using OpenBot.ConfigurationClasses.ModuleConfig;

namespace OpenBot.Modules;

internal class AuthenticationModule : PrivateModule
{
    internal AuthenticationModule(ModuleContext context) : base(context)
    {
    }

    internal string GetToken()
    {
        AuthenticationConfig config = this.GetConfig<AuthenticationConfig>();

        string token = config.BotToken;

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Discord token is empty. This is a configuration issue: check /ModuleConfig/Authentication/config.yml!");
        }

        // Discord tokens:
        //  are ~70 characters long
        //  contain 2 . characters (3 sections)
        if (token.Length < 50 || token.Split('.').Length != 3)
        {
            throw new InvalidOperationException("Discord token is invalid. This is a configuration issue: check /ModuleConfig/Authentication/config.yml!");
        }

        return token;
    }
}