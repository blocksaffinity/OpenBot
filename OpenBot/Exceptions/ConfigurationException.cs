using OpenBot.API.Modules;

namespace OpenBot.Exceptions;

internal class ConfigurationException : InvalidOperationException
{
    string SourceModuleName { get; }
    public ConfigurationException(string moduleName, string message, Exception? innerException = null) : base(message, innerException)
    {
        SourceModuleName = moduleName;
    }
}