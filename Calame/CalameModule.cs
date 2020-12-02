using System.ComponentModel.Composition;
using Gemini.Framework;

namespace Calame
{
    [Export(typeof(IModule))]
    public class CalameModule : ModuleBase
    {
        public override void Initialize()
        {
            Shell.ToolBars.Visible = true;
        }
    }
}