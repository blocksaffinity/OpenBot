namespace OpenBot.API;

// Jank?? Sure hope not
public static class APIDetails
{
    public const string ApiVersion = "0.1.0-beta";
    public readonly static bool ApiBetaVersion = ApiVersion.Contains("-beta");
}