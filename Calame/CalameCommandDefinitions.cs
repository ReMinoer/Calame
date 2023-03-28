using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.Shell.Commands;
using Gemini.Modules.UndoRedo.Commands;
using MenuDefinitions = Gemini.Modules.MainMenu.MenuDefinitions;

namespace Calame
{
    static public class ExcludedMenus
    {
        [Export]
        static public readonly ExcludeMenuDefinition ExcludeViewMenu = new ExcludeMenuDefinition(MenuDefinitions.ViewMenu);
        [Export]
        static public readonly ExcludeMenuDefinition ExcludeToolsMenu = new ExcludeMenuDefinition(MenuDefinitions.ToolsMenu);
        [Export]
        static public readonly ExcludeMenuDefinition ExcludeWindowMenu = new ExcludeMenuDefinition(MenuDefinitions.WindowMenu);
    }

    static public class CalameToolBars
    {
        [Export]
        static public readonly ToolBarItemDefinition Reopen = new CommandToolBarItemDefinition<ReopenCommand>(Gemini.Modules.Shell.ToolBarDefinitions.StandardOpenSaveToolBarGroup, 100);

        [Export]
        static public readonly ToolBarDefinition ExecuteToolBar = new ToolBarDefinition(1000, "Execute");
        [Export]
        static public readonly ToolBarItemGroupDefinition RunGroup = new ToolBarItemGroupDefinition(ExecuteToolBar, 0);
        [Export]
        static public readonly ToolBarItemDefinition RunDocument = new CommandToolBarItemDefinition<RunDocumentCommand>(RunGroup, 0, ToolBarItemDisplay.IconAndText);
        [Export]
        static public readonly ToolBarItemDefinition RunDocumentAlternative = new CommandToolBarItemDefinition<RunDocumentAlternativeCommand>(RunGroup, 10);
    }

    static public class CalameMenus
    {
        [Export]
        static public readonly MenuItemDefinition Reopen = new CommandMenuItemDefinition<ReopenCommand>(MenuDefinitions.FileCloseMenuGroup, -100);

        [Export]
        static public readonly MenuItemGroupDefinition FileSystemGroup = new MenuItemGroupDefinition(MenuDefinitions.FileMenu, 9);
        //[Export]
        //static public readonly MenuItemDefinition Options = new CommandMenuItemDefinition<OpenSettingsCommandDefinition>(FileSystemGroup, 0);

        [Export]
        static public readonly MenuItemGroupDefinition SelectionHistoryGroup = new MenuItemGroupDefinition(MenuDefinitions.EditMenu, 100);
        [Export]
        static public readonly MenuItemDefinition PreviousSelection = new CommandMenuItemDefinition<PreviousSelectionCommand>(SelectionHistoryGroup, 0);
        [Export]
        static public readonly MenuItemDefinition NextSelection = new CommandMenuItemDefinition<NextSelectionCommand>(SelectionHistoryGroup, 0);

        [Export]
        static public readonly MenuDefinition ExecuteMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 100, "E_xecute");
        [Export]
        static public readonly MenuItemGroupDefinition ExecuteRunGroup = new MenuItemGroupDefinition(ExecuteMenu, 0);
        [Export]
        static public readonly MenuItemDefinition RunDocument = new CommandMenuItemDefinition<RunDocumentCommand>(ExecuteRunGroup, 0);
        [Export]
        static public readonly MenuItemDefinition RunDocumentAlternative = new CommandMenuItemDefinition<RunDocumentAlternativeCommand>(ExecuteRunGroup, 10);

        [Export]
        static public readonly MenuDefinition WindowMenu = new MenuDefinition(MenuDefinitions.MainMenuBar, 200, "_Window");
        [Export]
        static public readonly MenuItemGroupDefinition WindowLayoutGroup = new MenuItemGroupDefinition(WindowMenu, 0);
        [Export]
        static public readonly MenuItemGroupDefinition WindowPresetsGroup = new MenuItemGroupDefinition(WindowMenu, 100);
        [Export]
        static public readonly MenuItemGroupDefinition WindowToolsGroup = new MenuItemGroupDefinition(WindowMenu, 200);
        [Export]
        static public readonly MenuItemGroupDefinition WindowDocumentsGroup = new MenuItemGroupDefinition(WindowMenu, 300);

        [Export]
        static public readonly MenuItemDefinition ReloadLayout = new CommandMenuItemDefinition<ReloadLayoutCommand>(WindowLayoutGroup, 0);
        [Export]
        static public readonly MenuItemDefinition SaveLayout = new CommandMenuItemDefinition<SaveLayoutCommand>(WindowLayoutGroup, 10);
        [Export]
        static public readonly MenuItemDefinition DefaultPresetCommand = new CommandMenuItemDefinition<DefaultPresetCommand>(WindowPresetsGroup, 0);
        [Export]
        static public readonly MenuItemDefinition ViewHistoryCommand = new CommandMenuItemDefinition<ViewHistoryCommandDefinition>(WindowToolsGroup, 0);
        [Export]
        static public readonly MenuItemDefinition WindowDocumentList = new CommandMenuItemDefinition<SwitchToDocumentCommandListDefinition>(WindowDocumentsGroup, 0);
    }

    static public class CalameShortcuts
    {
        [Export]
        static public readonly CommandKeyboardShortcut PreviousSelection = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.PageUp, ModifierKeys.Control));
        [Export]
        static public readonly CommandKeyboardShortcut PreviousSelectionBrowser = new CommandKeyboardShortcut<PreviousSelectionCommand>(new KeyGesture(Key.BrowserBack));
        [Export]
        static public readonly CommandKeyboardShortcut NextSelection = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.PageDown, ModifierKeys.Control));
        [Export]
        static public readonly CommandKeyboardShortcut NextSelectionBrowser = new CommandKeyboardShortcut<NextSelectionCommand>(new KeyGesture(Key.BrowserForward));

        [Export]
        static public readonly CommandKeyboardShortcut RunDocument = new CommandKeyboardShortcut<RunDocumentCommand>(new KeyGesture(Key.F5));
        [Export]
        static public readonly CommandKeyboardShortcut RunDocumentAlternative = new CommandKeyboardShortcut<RunDocumentAlternativeCommand>(new KeyGesture(Key.F5, ModifierKeys.Alt));
    }
}