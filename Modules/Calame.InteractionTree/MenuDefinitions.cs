using System.ComponentModel.Composition;
using Calame.InteractionTree.Commands;
using Gemini.Framework.Menus;

namespace Calame.InteractionTree
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition CompositionGraphCommandMenuItem = new CommandMenuItemDefinition<InteractionTreeCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}