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
        static public readonly ToolBarDefinition Definition = new ToolBarDefinition(100, "Viewer");

        [Export]
        static public readonly ToolBarItemGroupDefinition ModesGroup = new ToolBarItemGroupDefinition(Definition, 0);
        [Export]
        static public readonly ToolBarItemDefinition EditorMode = new CommandToolBarItemDefinition<EditorModeCommand>(ModesGroup, 0);

        [Export]
        static public readonly ToolBarItemGroupDefinition DebugGroup = new ToolBarItemGroupDefinition(Definition, 100);
        [Export]
        static public readonly ToolBarItemDefinition ViewerDebugMode = new CommandToolBarItemDefinition<ViewerDebugModeCommand>(DebugGroup, 0);

        [Export]
        static public readonly ToolBarItemGroupDefinition CameraActionGroup = new ToolBarItemGroupDefinition(Definition, 200);
        [Export]
        static public readonly ToolBarItemDefinition ResetCamera = new CommandToolBarItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public readonly ToolBarItemDefinition FocusCamera = new CommandToolBarItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    public class ViewerMenu
    {
        [Export]
        static public readonly MenuDefinition Definition = new MenuDefinition(MenuDefinitions.MainMenuBar, 150, "_Viewer");

        [Export]
        static public readonly MenuItemGroupDefinition ModesGroup = new MenuItemGroupDefinition(Definition, 0);
        [Export]
        static public readonly MenuItemDefinition EditorMode = new CommandMenuItemDefinition<EditorModeCommand>(ModesGroup, 0);

        [Export]
        static public readonly MenuItemGroupDefinition DebugGroup = new MenuItemGroupDefinition(Definition, 100);
        [Export]
        static public readonly MenuItemDefinition ViewerDebugMode = new CommandMenuItemDefinition<ViewerDebugModeCommand>(DebugGroup, 0);

        [Export]
        static public readonly MenuItemGroupDefinition EditSelectionGroup = new MenuItemGroupDefinition(MenuDefinitions.EditMenu, 200);
        [Export]
        static public readonly MenuItemDefinition DeleteSelection = new CommandMenuItemDefinition<DeleteSelectionCommand>(EditSelectionGroup, 0);

        [Export]
        static public readonly MenuItemGroupDefinition CameraActionGroup = new MenuItemGroupDefinition(Definition, 200);
        [Export]
        static public readonly MenuItemDefinition ResetCamera = new CommandMenuItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public readonly MenuItemDefinition FocusCamera = new CommandMenuItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public readonly CommandKeyboardShortcut EditorMode = new CommandKeyboardShortcut<EditorModeCommand>(new KeyGesture(Key.F1, ModifierKeys.Control));

        [Export]
        static public readonly CommandKeyboardShortcut ViewerDebugMode = new CommandKeyboardShortcut<ViewerDebugModeCommand>(new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift));

        [Export]
        static public readonly CommandKeyboardShortcut DeleteSelection = new CommandKeyboardShortcut<DeleteSelectionCommand>(new KeyGesture(Key.Delete));

        [Export]
        static public readonly CommandKeyboardShortcut ResetCamera = new CommandKeyboardShortcut<ResetCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Alt));
        [Export]
        static public readonly CommandKeyboardShortcut FocusCamera = new CommandKeyboardShortcut<FocusCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control));
    }
}