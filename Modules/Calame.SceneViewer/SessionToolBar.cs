using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.SceneViewer.Commands;
using Calame.Viewer;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;

namespace Calame.SceneViewer
{
    static public class SessionToolBar
    {
        [Export]
        static public ToolBarItemGroupDefinition EngineGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, -20);
        [Export]
        static public ToolBarItemDefinition EnginePauseResume = new CommandToolBarItemDefinition<EnginePauseResumeCommand>(EngineGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        static public ToolBarItemGroupDefinition CameraGroup = new ToolBarItemGroupDefinition(ViewerToolBar.Definition, -10);
        [Export]
        static public ToolBarItemDefinition FreeCamera = new CommandToolBarItemDefinition<FreeCameraCommand>(CameraGroup, 0);
        [Export]
        static public ToolBarItemDefinition DefaultCamera = new CommandToolBarItemDefinition<DefaultCameraCommand>(CameraGroup, 10);

        [Export]
        static public ToolBarItemDefinition SessionMode = new CommandToolBarItemDefinition<SessionModeCommand>(ViewerToolBar.ModeGroup, -10);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut EngineResumePause = new CommandKeyboardShortcut<EnginePauseResumeCommand>(new KeyGesture(Key.F1));
        [Export]
        static public CommandKeyboardShortcut FreeCamera = new CommandKeyboardShortcut<FreeCameraCommand>(new KeyGesture(Key.F2));
        [Export]
        static public CommandKeyboardShortcut DefaultCamera = new CommandKeyboardShortcut<DefaultCameraCommand>(new KeyGesture(Key.F3));
        [Export]
        static public CommandKeyboardShortcut SessionMode = new CommandKeyboardShortcut<SessionModeCommand>(new KeyGesture(Key.F4));
    }
}