using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework;

namespace Calame.DataModelTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(DataModelTreeViewModel);
            }
        }
    }
}