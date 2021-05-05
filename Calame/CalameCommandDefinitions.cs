using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;

namespace Calame
{
    public class CalameMenu
    {
        [Export]
        static public MenuItemGroupDefinition ToolsSubMenuGroup = new MenuItemGroupDefinition(MenuDefinitions.ToolsMenu, 0);

        [Export]
        static public MenuItemGroupDefinition SelectionHistoryGroup = new MenuItemGroupDefinition(MenuDefinitions.EditMenu, 10);
        [Export]
        static public MenuItemDefinition PreviousSelection = new CommandMenuItemDefinition<PreviousSelectionCommand>(SelectionHistoryGroup, 0);
        [Export]
        static public MenuItemDefinition NextSelection = new CommandMenuItemDefinition<NextSelectionCommand>(SelectionHistoryGroup, 0);
    }

    static public class CalameShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut PreviousSelection = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.Back, ModifierKeys.Alt));
        [Export]
        static public CommandKeyboardShortcut NextSelection = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.Back, ModifierKeys.Alt | ModifierKeys.Shift));
    }
}