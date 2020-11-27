using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.SceneViewer.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;

namespace Calame.SceneViewer
{
    static public class SessionToolBar
    {
        [Export]
        static public ToolBarDefinition Definition = new ToolBarDefinition(0, "Session");

        [Export]
        static public ToolBarItemGroupDefinition EngineGroup = new ToolBarItemGroupDefinition(Definition, 0);
        [Export]
        static public ToolBarItemDefinition EnginePauseResume = new CommandToolBarItemDefinition<EnginePauseResumeCommand>(EngineGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        static public ToolBarItemGroupDefinition CameraGroup = new ToolBarItemGroupDefinition(Definition, 2);
        [Export]
        static public ToolBarItemDefinition FreeCamera = new CommandToolBarItemDefinition<FreeCameraCommand>(CameraGroup, 0);
        [Export]
        static public ToolBarItemDefinition DefaultCamera = new CommandToolBarItemDefinition<DefaultCameraCommand>(CameraGroup, 2);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut EngineResumePauseShortcut = new CommandKeyboardShortcut<EnginePauseResumeCommand>(new KeyGesture(Key.Pause));
        [Export]
        static public CommandKeyboardShortcut FreeCameraShortcut = new CommandKeyboardShortcut<FreeCameraCommand>(new KeyGesture(Key.F1));
        [Export]
        static public CommandKeyboardShortcut DefaultCameraShortcut = new CommandKeyboardShortcut<DefaultCameraCommand>(new KeyGesture(Key.F2));
    }
}