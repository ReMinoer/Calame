using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.SceneViewer.Commands;
using Calame.Viewer;
using Calame.Viewer.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;

namespace Calame.SceneViewer
{
    static public class SessionToolBar
    {
        [Export]
        static public ToolBarItemDefinition SessionMode = new CommandToolBarItemDefinition<SessionModeCommand>(ViewerToolBar.ModesGroup, 20);

        [Export]
        static public ToolBarDefinition Definition = new ToolBarDefinition(200, "Session");

        [Export]
        static public ToolBarItemGroupDefinition EngineGroup = new ToolBarItemGroupDefinition(Definition, 100);
        [Export]
        static public ToolBarItemDefinition EnginePause = new CommandToolBarItemDefinition<EnginePauseCommand>(EngineGroup, 0);
        [Export]
        static public ToolBarItemDefinition EngineNextFrame = new CommandToolBarItemDefinition<NextFrameCommand>(EngineGroup, 10);

        [Export]
        static public ToolBarItemGroupDefinition CameraModeGroup = new ToolBarItemGroupDefinition(Definition, 110);
        [Export]
        static public ToolBarItemDefinition FreeCamera = new CommandToolBarItemDefinition<FreeCameraCommand>(CameraModeGroup, 0);
        [Export]
        static public ToolBarItemDefinition DefaultCamera = new CommandToolBarItemDefinition<DefaultCameraCommand>(CameraModeGroup, 10);

        [Export]
        static public ToolBarItemGroupDefinition CameraActionGroup = new ToolBarItemGroupDefinition(Definition, 120);
        [Export]
        static public ToolBarItemDefinition ResetCamera = new CommandToolBarItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public ToolBarItemDefinition FocusCamera = new CommandToolBarItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    static public class SessionMenu
    {
        [Export]
        static public MenuItemDefinition SessionMode = new CommandMenuItemDefinition<SessionModeCommand>(ViewerMenu.ModesGroup, 20);

        [Export]
        static public MenuItemGroupDefinition EngineGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 100);
        [Export]
        static public MenuItemDefinition EnginePause = new CommandMenuItemDefinition<EnginePauseCommand>(EngineGroup, 0);
        [Export]
        static public MenuItemDefinition EngineNextFrame = new CommandMenuItemDefinition<NextFrameCommand>(EngineGroup, 10);

        [Export]
        static public MenuItemGroupDefinition CameraModeGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 110);
        [Export]
        static public MenuItemDefinition FreeCamera = new CommandMenuItemDefinition<FreeCameraCommand>(CameraModeGroup, 0);
        [Export]
        static public MenuItemDefinition DefaultCamera = new CommandMenuItemDefinition<DefaultCameraCommand>(CameraModeGroup, 10);

        [Export]
        static public MenuItemGroupDefinition CameraActionGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 110);
        [Export]
        static public MenuItemDefinition ResetCamera = new CommandMenuItemDefinition<ResetCameraCommand>(CameraActionGroup, 0);
        [Export]
        static public MenuItemDefinition FocusCamera = new CommandMenuItemDefinition<FocusCameraCommand>(CameraActionGroup, 10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut SessionMode = new CommandKeyboardShortcut<SessionModeCommand>(new KeyGesture(Key.F3));

        [Export]
        static public CommandKeyboardShortcut EnginePause = new CommandKeyboardShortcut<EnginePauseCommand>(new KeyGesture(Key.F6));
        [Export]
        static public CommandKeyboardShortcut EngineNextFrame = new CommandKeyboardShortcut<NextFrameCommand>(new KeyGesture(Key.F7));

        [Export]
        static public CommandKeyboardShortcut FreeCamera = new CommandKeyboardShortcut<FreeCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Alt));
        [Export]
        static public CommandKeyboardShortcut DefaultCamera = new CommandKeyboardShortcut<DefaultCameraCommand>(new KeyGesture(Key.D, ModifierKeys.Alt));

        [Export]
        static public CommandKeyboardShortcut ResetCamera = new CommandKeyboardShortcut<ResetCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Alt));
        [Export]
        static public CommandKeyboardShortcut FocusCamera = new CommandKeyboardShortcut<FocusCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Control));
    }
}