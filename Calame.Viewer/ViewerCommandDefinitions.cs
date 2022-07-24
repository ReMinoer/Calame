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

        [Export]
        static public ToolBarItemGroupDefinition DebugGroup = new ToolBarItemGroupDefinition(Definition, 100);
        [Export]
        static public ToolBarItemDefinition ViewerDebugMode = new CommandToolBarItemDefinition<ViewerDebugModeCommand>(DebugGroup, 0);

        [Export]
        static public ToolBarItemGroupDefinition CameraActionGroup = new ToolBarItemGroupDefinition(Definition, 200);
        [Export]
        static public ToolBarItemDefinition ResetCamera = new CommandToolBarItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public ToolBarItemDefinition FocusCamera = new CommandToolBarItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    public class ViewerMenu
    {
        [Export]
        static public MenuDefinition Definition = new MenuDefinition(MenuDefinitions.MainMenuBar, 150, "_Viewer");

        [Export]
        static public MenuItemGroupDefinition ModesGroup = new MenuItemGroupDefinition(Definition, 0);
        [Export]
        static public MenuItemDefinition EditorMode = new CommandMenuItemDefinition<EditorModeCommand>(ModesGroup, 0);

        [Export]
        static public MenuItemGroupDefinition DebugGroup = new MenuItemGroupDefinition(Definition, 100);
        [Export]
        static public MenuItemDefinition ViewerDebugMode = new CommandMenuItemDefinition<ViewerDebugModeCommand>(DebugGroup, 0);

        [Export]
        static public MenuItemGroupDefinition EditSelectionGroup = new MenuItemGroupDefinition(MenuDefinitions.EditMenu, 200);
        [Export]
        static public MenuItemDefinition DeleteSelection = new CommandMenuItemDefinition<DeleteSelectionCommand>(EditSelectionGroup, 0);

        [Export]
        static public MenuItemGroupDefinition CameraActionGroup = new MenuItemGroupDefinition(Definition, 200);
        [Export]
        static public MenuItemDefinition ResetCamera = new CommandMenuItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public MenuItemDefinition FocusCamera = new CommandMenuItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut EditorMode = new CommandKeyboardShortcut<EditorModeCommand>(new KeyGesture(Key.F1));

        [Export]
        static public CommandKeyboardShortcut ViewerDebugMode = new CommandKeyboardShortcut<ViewerDebugModeCommand>(new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift));

        [Export]
        static public CommandKeyboardShortcut DeleteSelection = new CommandKeyboardShortcut<DeleteSelectionCommand>(new KeyGesture(Key.Delete));

        [Export]
        static public CommandKeyboardShortcut ResetCamera = new CommandKeyboardShortcut<ResetCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Alt));
        [Export]
        static public CommandKeyboardShortcut FocusCamera = new CommandKeyboardShortcut<FocusCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control));
    }
}