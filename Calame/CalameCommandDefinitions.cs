﻿using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.MainMenu;

namespace Calame
{
    static public class CalameToolBars
    {
        [Export]
        static public ToolBarDefinition ExecuteToolBar = new ToolBarDefinition(1000, "Execute");
        [Export]
        static public ToolBarItemGroupDefinition RunGroup = new ToolBarItemGroupDefinition(ExecuteToolBar, 0);
        [Export]
        static public ToolBarItemDefinition RunDocument = new CommandToolBarItemDefinition<RunDocumentCommand>(RunGroup, 0, ToolBarItemDisplay.IconAndText);
    }

    static public class CalameMenus
    {
        [Export]
        static public MenuDefinition ExecuteMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 6, "Execute");
        [Export]
        static public MenuItemGroupDefinition RunGroup = new MenuItemGroupDefinition(ExecuteMenu, 0);
        [Export]
        static public MenuItemDefinition RunDocument = new CommandMenuItemDefinition<RunDocumentCommand>(RunGroup, 0);

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
        static public CommandKeyboardShortcut RunSession = new CommandKeyboardShortcut<RunDocumentCommand>(new KeyGesture(Key.F5));

        [Export]
        static public CommandKeyboardShortcut PreviousSelection = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.Back, ModifierKeys.Alt));
        [Export]
        static public CommandKeyboardShortcut NextSelection = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.Back, ModifierKeys.Alt | ModifierKeys.Shift));
    }
}