using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework;

namespace Calame.SceneGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(SceneGraphViewModel);
            }
        }
    }
}