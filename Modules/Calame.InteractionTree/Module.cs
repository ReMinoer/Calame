using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.InteractionTree.ViewModels;
using Gemini.Framework;

namespace Calame.InteractionTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(InteractionTreeViewModel);
            }
        }
    }
}