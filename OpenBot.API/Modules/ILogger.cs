namespace OpenBot.API.Modules;

public interface ILogger
{
    void Debug(string text);
    void Info(string text);
    void Hint(string text);
    void Warn(string text);
    void Error(string text);
    void Fatal(string text);
}