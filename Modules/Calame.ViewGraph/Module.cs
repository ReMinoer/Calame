using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.ViewGraph.Commands;
using Calame.ViewGraph.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Calame.ViewGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public readonly MenuItemDefinition MenuItem = new CommandMenuItemDefinition<ViewGraphCommand>(CalameMenus.WindowToolsGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(ViewGraphViewModel);
            }
        }
    }
}