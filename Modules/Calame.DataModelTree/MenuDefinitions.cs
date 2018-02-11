using System.ComponentModel.Composition;
using Calame.DataModelTree.Commands;
using Gemini.Framework.Menus;

namespace Calame.DataModelTree
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition CompositionGraphCommandMenuItem = new CommandMenuItemDefinition<DataModelTreeCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}