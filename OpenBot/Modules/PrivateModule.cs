using OpenBot.API;
using OpenBot.API.Modules;

namespace OpenBot.Modules;

internal abstract class PrivateModule : Module
{
    protected PrivateModule(ModuleContext context) : base(context)
    {
    }
}