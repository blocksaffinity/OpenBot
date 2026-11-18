namespace OpenBot.Utils;

public static class FileUtils
{
    /// <summary>
    /// Adds the provided line to a file at the provided path
    /// </summary>
    /// <param name="line">The line to append to the file</param>
    /// <param name="path">The file path</param>
    public static void AddLineToFile(string line, string path)
    {
        EnsureFileExists(path);
        File.AppendAllText(path, line + Environment.NewLine);
    }

    /// <summary>
    /// Ensures a file at the given path exists
    /// </summary>
    /// <param name="filePath">The target file</param>
    public static void EnsureFileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            EnsureDirectoryExists(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, string.Empty);
        }
    }

    /// <summary>
    /// Ensures a directory of the given path exists
    /// </summary>
    /// <param name="directoryPath">The target directory</param>
    public static void EnsureDirectoryExists(string? directoryPath)
    {
        if (!Directory.Exists(directoryPath) && !string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}