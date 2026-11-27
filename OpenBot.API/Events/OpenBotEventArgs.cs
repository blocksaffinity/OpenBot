using OpenBot.API.Enums;
using OpenBot.API.Modules;

namespace OpenBot.API.Events;

public sealed class ModuleStartedEventArgs : EventArgs
{
    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The Module which has been started
    /// </summary>
    public Module Module { get; }

    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The name of the module which has been started
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The status of the module
    /// </summary>
    public ModuleStatus Status { get; }

    
    public ModuleStartedEventArgs(Module module)
    {
        this.Module = module;
        this.Name = module.Name;
        this.Status = module.Status;
    }
}

public sealed class ExtensionStartedEventArgs : EventArgs
{
    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The Extension which has been started
    /// </summary>
    public Extension Extension { get; }

    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The name of the extension which has been started
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// [0.2.0-beta]<br/>
    /// The status of the extension
    /// </summary>
    public ExtensionStatus Status { get; }


    public ExtensionStartedEventArgs(Extension extension)
    {
        this.Extension = extension;
        this.Name = extension.Name;
        this.Status = extension.Status;
    }
}