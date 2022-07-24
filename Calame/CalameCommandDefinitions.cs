using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.Shell.Commands;
using Gemini.Modules.UndoRedo;
using Gemini.Modules.UndoRedo.Commands;
using MenuDefinitions = Gemini.Modules.MainMenu.MenuDefinitions;

namespace Calame
{
    static public class ExcludedMenus
    {
        [Export]
        static public ExcludeMenuDefinition ExcludeViewMenu = new ExcludeMenuDefinition(MenuDefinitions.ViewMenu);
        [Export]
        static public ExcludeMenuDefinition ExcludeToolsMenu = new ExcludeMenuDefinition(MenuDefinitions.ToolsMenu);
        [Export]
        static public ExcludeMenuDefinition ExcludeWindowMenu = new ExcludeMenuDefinition(MenuDefinitions.WindowMenu);
    }

    static public class CalameToolBars
    {
        [Export]
        static public ToolBarDefinition ExecuteToolBar = new ToolBarDefinition(1000, "Execute");
        [Export]
        static public ToolBarItemGroupDefinition RunGroup = new ToolBarItemGroupDefinition(ExecuteToolBar, 0);
        [Export]
        static public ToolBarItemDefinition RunDocument = new CommandToolBarItemDefinition<RunDocumentCommand>(RunGroup, 0, ToolBarItemDisplay.IconAndText);
        [Export]
        static public ToolBarItemDefinition RunDocumentAlternative = new CommandToolBarItemDefinition<RunDocumentAlternativeCommand>(RunGroup, 10);
    }

    static public class CalameMenus
    {
        [Export]
        static public MenuItemGroupDefinition SelectionHistoryGroup = new MenuItemGroupDefinition(MenuDefinitions.EditMenu, 100);
        [Export]
        static public MenuItemDefinition PreviousSelection = new CommandMenuItemDefinition<PreviousSelectionCommand>(SelectionHistoryGroup, 0);
        [Export]
        static public MenuItemDefinition NextSelection = new CommandMenuItemDefinition<NextSelectionCommand>(SelectionHistoryGroup, 0);

        [Export]
        static public MenuDefinition ExecuteMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 100, "E_xecute");
        [Export]
        static public MenuItemGroupDefinition RunGroup = new MenuItemGroupDefinition(ExecuteMenu, 0);
        [Export]
        static public MenuItemDefinition RunDocument = new CommandMenuItemDefinition<RunDocumentCommand>(RunGroup, 0);
        [Export]
        static public MenuItemDefinition RunDocumentAlternative = new CommandMenuItemDefinition<RunDocumentAlternativeCommand>(RunGroup, 10);

        [Export]
        static public MenuDefinition WindowMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 200, "_Window");
        [Export]
        static public MenuItemGroupDefinition WindowLayoutGroup = new MenuItemGroupDefinition(WindowMenu, 0);
        [Export]
        static public MenuItemGroupDefinition WindowPresetsGroup = new MenuItemGroupDefinition(WindowMenu, 100);
        [Export]
        static public MenuItemGroupDefinition WindowToolsGroup = new MenuItemGroupDefinition(WindowMenu, 200);
        [Export]
        static public MenuItemGroupDefinition WindowDocumentsGroup = new MenuItemGroupDefinition(WindowMenu, 300);

        [Export]
        static public MenuItemDefinition ReloadLayout = new CommandMenuItemDefinition<ReloadLayoutCommand>(WindowLayoutGroup, 0);
        [Export]
        static public MenuItemDefinition SaveLayout = new CommandMenuItemDefinition<SaveLayoutCommand>(WindowLayoutGroup, 10);
        [Export]
        static public MenuItemDefinition DefaultPresetCommand = new CommandMenuItemDefinition<DefaultPresetCommand>(WindowPresetsGroup, 0);
        [Export]
        static public MenuItemDefinition ViewHistoryCommand = new CommandMenuItemDefinition<ViewHistoryCommandDefinition>(WindowToolsGroup, 0);
        [Export]
        public static MenuItemDefinition WindowDocumentList = new CommandMenuItemDefinition<SwitchToDocumentCommandListDefinition>(WindowDocumentsGroup, 0);
    }

    static public class CalameShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut PreviousSelection = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.PageUp, ModifierKeys.Control));
        [Export]
        static public CommandKeyboardShortcut PreviousSelectionBrowser = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.BrowserBack));
        [Export]
        static public CommandKeyboardShortcut NextSelection = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.PageDown, ModifierKeys.Control));
        [Export]
        static public CommandKeyboardShortcut NextSelectionBrowser = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.BrowserForward));

        [Export]
        static public CommandKeyboardShortcut RunDocument = new CommandKeyboardShortcut<RunDocumentCommand>(new KeyGesture(Key.F5));
        [Export]
        static public CommandKeyboardShortcut RunDocumentAlternative = new CommandKeyboardShortcut<RunDocumentAlternativeCommand>(new KeyGesture(Key.F5, ModifierKeys.Alt));
    }
}