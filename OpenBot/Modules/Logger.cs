using System.Collections.Concurrent;
using OpenBot.API.Modules;
using OpenBot.Utils;

namespace OpenBot.Modules;

internal static class LoggingService
{
    internal static readonly DateTime StartTime = DateTime.Now;

    internal static readonly string LogFilePath =
        Path.Combine(API.OpenBot.GetRootDirectory(), "logs", $"{StartTime:yyyy-MM-dd_HH-mm-ss}.log");

    internal static readonly ConcurrentDictionary<string, Logger> Loggers = [];

    internal static Logger GetLogger(Module associatedModule)
    {
        return Loggers.GetOrAdd(associatedModule.Name, key => new Logger(associatedModule, LogFilePath));
    }

    internal static Logger GetLogger(string name)
    {
        return Loggers.GetOrAdd(name, key => new Logger(name, LogFilePath));
    }
}

public class Logger : ILogger
{
    internal string LogFilePath { get; }
    internal string Name { get; }
    internal Module? AssociatedModule { get; }

    internal Logger(Module associatedModule , string logFilePath)
    {
        this.Name = associatedModule.Name;
        this.LogFilePath = logFilePath;
        this.AssociatedModule = associatedModule;
    }

    internal Logger(string name , string logFilePath)
    {
        this.Name = name;
        this.LogFilePath = logFilePath;
    }

    public void Debug(string text) => Log("DEBUG", "&8&l", text);
    public void Info(string text) => Log("INFO", "&9&l", text);
    public void Hint(string text) => Log("HINT", "&e&l", text);
    public void Warn(string text) => Log("WARN", "&6&l", text);
    public void Error(string text) => Log("ERROR", "&c&l", text);
    public void Fatal(string text) => Log("FATAL", "&4&l", text);

    private void Log(string level, string colour, string text)
    {
        DateTime now = DateTime.Now;

        ConsoleUtils.WriteLine(
            $"&7{now:yyyy-MM-dd HH:mm:ss} [{Name,-15}] {colour}{level,-5} &r{text}"
        );
        FileUtils.AddLineToFile($"{now:yyyy-MM-dd HH:mm:ss} {level,-5} {text}", LogFilePath);
    }
}