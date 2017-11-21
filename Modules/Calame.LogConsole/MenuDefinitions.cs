using System.ComponentModel.Composition;
using Calame.LogConsole.Commands;
using Gemini.Framework.Menus;

namespace Calame.LogConsole
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition LogConsoleCommandMenuItem = new CommandMenuItemDefinition<LogConsoleCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}