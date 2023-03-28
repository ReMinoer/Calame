using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.CompositionGraph.Commands;
using Calame.CompositionGraph.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Calame.CompositionGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public readonly MenuItemDefinition MenuItem = new CommandMenuItemDefinition<CompositionGraphCommand>(CalameMenus.WindowToolsGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(CompositionGraphViewModel);
            }
        }
    }
}