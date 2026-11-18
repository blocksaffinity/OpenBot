namespace OpenBot.API.Modules;

/// <summary>
/// Base class for an OpenBot extension
/// </summary>
public abstract class Extension : Module
{
    /// <summary>
    /// Creates an instance of Extension
    /// </summary>
    /// <param name="context">Context for the extension</param>
    protected Extension(ModuleContext context) : base(context)
    {
        
    }
}