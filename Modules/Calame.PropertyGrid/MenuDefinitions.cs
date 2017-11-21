using System.ComponentModel.Composition;
using Calame.PropertyGrid.Commands;
using Gemini.Framework.Menus;

namespace Calame.PropertyGrid
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition PropertyGridCommandMenuItem = new CommandMenuItemDefinition<PropertyGridCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}