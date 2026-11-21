namespace OpenBot.API.Extensions;

public sealed class ExtensionMetadata
{
    public string? name;
    public string? file;
    public SemanticVersion? version;
    public SemanticVersion? apiVersion;
}