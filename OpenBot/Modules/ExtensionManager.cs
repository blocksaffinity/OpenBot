namespace OpenBot.Modules;

internal sealed class ExtensionManager
{
    private readonly Logger Logger = LoggingService.GetLogger("extensions");

    private readonly string ExtensionsDirectory =
        Path.Combine(AppContext.BaseDirectory, "extensions");

    internal string[] Discover()
    {
        Directory.CreateDirectory(ExtensionsDirectory);

        var files = Directory.GetFiles(
            ExtensionsDirectory,
            "*.dll",
            SearchOption.TopDirectoryOnly
        );

        Logger.Info($"Found {files.Length} extension DLL(s).");

        foreach (var file in files)
        {
            Logger.Info($"Discovered: {Path.GetFileName(file)}");
        }

        return files;
    }
}
