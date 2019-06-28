using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.BrushPanel.ViewModels;
using Gemini.Framework;

namespace Calame.BrushPanel
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(BrushPanelViewModel);
            }
        }
    }
}