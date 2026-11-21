namespace OpenBot.API;

public static class OpenBot
{
    /// <summary>
    /// Retrieves the cross-platform root directory of the bot
    /// </summary>
    /// <returns>The absolute path to the root directory of the bot</returns>
    public static string GetRootDirectory()
    {
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "blocksaffinity",
            "OpenBot"
        );

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new DirectoryNotFoundException("Failed to get local application folder directory.");
        }

        return path;
    }
}