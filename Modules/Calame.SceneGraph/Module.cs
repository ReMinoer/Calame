using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.SceneGraph.Commands;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Calame.SceneGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public readonly MenuItemDefinition MenuItem = new CommandMenuItemDefinition<SceneGraphCommand>(CalameMenus.WindowToolsGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(SceneGraphViewModel);
            }
        }
    }
}