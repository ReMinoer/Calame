using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.BrushPanel.Commands;
using Calame.BrushPanel.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;

namespace Calame.BrushPanel
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<BrushPanelCommand>(MenuDefinitions.ViewToolsMenuGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(BrushPanelViewModel);
            }
        }
    }
}