using System.ComponentModel.Composition;
using Gemini.Framework;
using Glyph;
using Microsoft.Xna.Framework;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public Module()
        {
            VirtualResolution.Size = new Vector2(1920, 1080);
        }
    }
}