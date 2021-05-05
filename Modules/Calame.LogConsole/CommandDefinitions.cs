using System.ComponentModel.Composition;
using Calame.LogConsole.Commands;
using Gemini.Framework.Menus;

namespace Calame.LogConsole
{
    static public class SessionMenu
    {
        [Export]
        static public MenuItemDefinition LogConsoleMenu = new TextMenuItemDefinition(CalameMenu.ToolsSubMenuGroup, 0, "_Log Console");
        [Export]
        static public MenuItemGroupDefinition LogConsoleGroup = new MenuItemGroupDefinition(LogConsoleMenu, 0);

        [Export]
        static public MenuItemDefinition ClearLog = new CommandMenuItemDefinition<ClearLogCommand>(LogConsoleGroup, 0);
        [Export]
        static public MenuItemDefinition AutoScrollLog = new CommandMenuItemDefinition<AutoScrollLogCommand>(LogConsoleGroup, 0);
        [Export]
        static public MenuItemDefinition ScrollLogToEnd = new CommandMenuItemDefinition<ScrollLogToEndCommand>(LogConsoleGroup, 0);
    }
}