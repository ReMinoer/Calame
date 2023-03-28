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
        static public readonly ToolBarItemDefinition SessionMode = new CommandToolBarItemDefinition<SessionModeCommand>(ViewerToolBar.ModesGroup, 20);

        [Export]
        static public readonly ToolBarItemGroupDefinition EngineGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, 150);
        [Export]
        static public readonly ToolBarItemDefinition EnginePause = new CommandToolBarItemDefinition<EnginePauseCommand>(EngineGroup, 0);
        [Export]
        static public readonly ToolBarItemDefinition EngineNextFrame = new CommandToolBarItemDefinition<NextFrameCommand>(EngineGroup, 10);

        [Export]
        static public readonly ToolBarItemGroupDefinition CameraModeGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, 160);
        [Export]
        static public readonly ToolBarItemDefinition FreeCamera = new CommandToolBarItemDefinition<FreeCameraCommand>(CameraModeGroup, 0);
        [Export]
        static public readonly ToolBarItemDefinition DefaultCamera = new CommandToolBarItemDefinition<DefaultCameraCommand>(CameraModeGroup, 10);
    }

    static public class SessionMenu
    {
        [Export]
        static public readonly MenuItemDefinition SessionMode = new CommandMenuItemDefinition<SessionModeCommand>(ViewerMenu.ModesGroup, 20);

        [Export]
        static public readonly MenuItemGroupDefinition EngineGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 150);
        [Export]
        static public readonly MenuItemDefinition EnginePause = new CommandMenuItemDefinition<EnginePauseCommand>(EngineGroup, 0);
        [Export]
        static public readonly MenuItemDefinition EngineNextFrame = new CommandMenuItemDefinition<NextFrameCommand>(EngineGroup, 10);

        [Export]
        static public readonly MenuItemGroupDefinition CameraModeGroup = new MenuItemGroupDefinition(ViewerMenu.Definition, 160);
        [Export]
        static public readonly MenuItemDefinition FreeCamera = new CommandMenuItemDefinition<FreeCameraCommand>(CameraModeGroup, 0);
        [Export]
        static public readonly MenuItemDefinition DefaultCamera = new CommandMenuItemDefinition<DefaultCameraCommand>(CameraModeGroup, 10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public readonly CommandKeyboardShortcut SessionMode = new CommandKeyboardShortcut<SessionModeCommand>(new KeyGesture(Key.F3, ModifierKeys.Control));

        [Export]
        static public readonly CommandKeyboardShortcut EnginePause = new CommandKeyboardShortcut<EnginePauseCommand>(new KeyGesture(Key.F6));
        [Export]
        static public readonly CommandKeyboardShortcut EngineNextFrame = new CommandKeyboardShortcut<NextFrameCommand>(new KeyGesture(Key.F7));

        [Export]
        static public readonly CommandKeyboardShortcut FreeCamera = new CommandKeyboardShortcut<FreeCameraCommand>(new KeyGesture(Key.F, ModifierKeys.Alt));
        [Export]
        static public readonly CommandKeyboardShortcut DefaultCamera = new CommandKeyboardShortcut<DefaultCameraCommand>(new KeyGesture(Key.D, ModifierKeys.Alt));
    }
}