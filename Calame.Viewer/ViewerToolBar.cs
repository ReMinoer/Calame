using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.MainMenu;

namespace Calame.Viewer
{
    public class ViewerToolBar
    {
        [Export]
        static public ToolBarDefinition Definition = new ToolBarDefinition(100, "Viewer");

        [Export]
        static public ToolBarItemGroupDefinition ModesGroup = new ToolBarItemGroupDefinition(Definition, 0);
        [Export]
        static public ToolBarItemDefinition EditorMode = new CommandToolBarItemDefinition<EditorModeCommand>(ModesGroup, 0);
    }

    public class ViewerMenu
    {
        [Export]
        static public MenuDefinition Definition = new MenuDefinition(MenuDefinitions.MainMenuBar, 100, "Viewer");

        [Export]
        static public MenuItemGroupDefinition ModesGroup = new MenuItemGroupDefinition(Definition, 0);
        [Export]
        static public MenuItemDefinition EditorMode = new CommandMenuItemDefinition<EditorModeCommand>(ModesGroup, 0);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut EditorMode = new CommandKeyboardShortcut<EditorModeCommand>(new KeyGesture(Key.F1));
    }
}