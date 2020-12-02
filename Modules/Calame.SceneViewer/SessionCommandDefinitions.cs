using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.SceneViewer.Commands;
using Calame.Viewer;
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
        static public ToolBarItemGroupDefinition SessionGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, 100);
        [Export]
        static public ToolBarItemDefinition EnginePause = new CommandToolBarItemDefinition<EnginePauseCommand>(SessionGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        static public ToolBarItemGroupDefinition CameraGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, 110);
        [Export]
        static public ToolBarItemDefinition FreeCamera = new CommandToolBarItemDefinition<FreeCameraCommand>(CameraGroup, 0);
        [Export]
        static public ToolBarItemDefinition DefaultCamera = new CommandToolBarItemDefinition<DefaultCameraCommand>(CameraGroup, 10);
    }

    static public class SessionMenu
    {
        [Export]
        static public MenuItemDefinition SessionMode = new CommandMenuItemDefinition<SessionModeCommand>(ViewerMenu.ModesGroup, 20);

        [Export]
        static public MenuItemGroupDefinition SessionGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 100);
        [Export]
        static public MenuItemDefinition EnginePause = new CommandMenuItemDefinition<EnginePauseCommand>(SessionGroup, 0);

        [Export]
        static public MenuItemGroupDefinition CameraGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 110);
        [Export]
        static public MenuItemDefinition FreeCamera = new CommandMenuItemDefinition<FreeCameraCommand>(CameraGroup, 0);
        [Export]
        static public MenuItemDefinition DefaultCamera = new CommandMenuItemDefinition<DefaultCameraCommand>(CameraGroup, 10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut SessionMode = new CommandKeyboardShortcut<SessionModeCommand>(new KeyGesture(Key.F3));

        [Export]
        static public CommandKeyboardShortcut EngineResumePause = new CommandKeyboardShortcut<EnginePauseCommand>(new KeyGesture(Key.F5));
        [Export]
        static public CommandKeyboardShortcut FreeCamera = new CommandKeyboardShortcut<FreeCameraCommand>(new KeyGesture(Key.F6));
        [Export]
        static public CommandKeyboardShortcut DefaultCamera = new CommandKeyboardShortcut<DefaultCameraCommand>(new KeyGesture(Key.F7));
    }
}