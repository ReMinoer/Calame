using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.PropertyGrid.Commands;
using Calame.PropertyGrid.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;

namespace Calame.PropertyGrid
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<PropertyGridCommand>(CalameMenus.WindowToolsGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(PropertyGridViewModel);
            }
        }
    }
}