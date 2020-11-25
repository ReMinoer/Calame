using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.InteractionTree.Commands;
using Calame.InteractionTree.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;

namespace Calame.InteractionTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<InteractionTreeCommand>(MenuDefinitions.ViewToolsMenuGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(InteractionTreeViewModel);
            }
        }
    }
}