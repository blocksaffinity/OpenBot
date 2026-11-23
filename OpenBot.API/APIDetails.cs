namespace OpenBot.API;

// Jank?? Sure hope not
public static class APIDetails
{
    public readonly static SemanticVersion ApiVersion = new(0, 1, 1, "-beta");
    public readonly static bool ApiPreReleaseVersion = ApiVersion.Label.Contains("beta") || ApiVersion.Label.Contains("alpha");
}