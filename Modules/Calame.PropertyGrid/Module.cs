using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.PropertyGrid.ViewModels;
using Gemini.Framework;

namespace Calame.PropertyGrid
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(PropertyGridViewModel);
            }
        }
    }
}