namespace OpenBot.API;

public sealed class SemanticVersion
{
    /// <summary>
    /// The major version
    /// </summary>
    public uint Major { get; }

    /// <summary>
    /// The minor version
    /// </summary>
    public uint Minor { get; }

    /// <summary>
    /// The patch version
    /// </summary>
    public uint Patch { get; }

    /// <summary>
    /// The release / version label (e.g. -beta)
    /// </summary>
    public string Label { get; } = string.Empty;

    // I hope this is not jank

    /// <summary>
    /// Initialise a new Version
    /// </summary>
    public SemanticVersion(uint major, uint minor, uint patch, string? label = null)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        Label = label ?? string.Empty;
    }

    public SemanticVersion(string version)
    {
        string[] split = version.Split('.', '-');

        if (split.Length != 3 && split.Length != 4)
        {
            throw new ArgumentException($"String version of SemanticVesion() must follow the format 'x.y.z' or x.y.z-abc' where x, y and z are uint: got {version}.");
        }

        try
        {
            Major = Convert.ToUInt32(split[0]);
            Minor = Convert.ToUInt32(split[1]);
            Patch = Convert.ToUInt32(split[2]);
        }
        catch (FormatException)
        {
            throw new ArgumentException($"String version of SemanticVersion() must have its major, minor and patch releases as uint, got {version}.");
        }

        if (split.Length == 4)
        {
            Label = split[3];
        }
    }
}