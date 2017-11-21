using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.CompositionGraph.ViewModels;
using Gemini.Framework;

namespace Calame.CompositionGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(CompositionGraphViewModel);
            }
        }
    }
}