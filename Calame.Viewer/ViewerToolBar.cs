using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.Viewer.Commands;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;

namespace Calame.Viewer
{
    public class ViewerToolBar
    {
        [Export]
        static public ToolBarDefinition Definition = new ToolBarDefinition(0, "Viewer");

        [Export]
        static public ToolBarItemGroupDefinition ModeGroup = new ToolBarItemGroupDefinition(Definition, 0);
        [Export]
        static public ToolBarItemDefinition EditorMode = new CommandToolBarItemDefinition<EditorModeCommand>(ModeGroup, 0);
    }

    static public class SessionShortcuts
    {
        [Export]
        static public CommandKeyboardShortcut EditorMode = new CommandKeyboardShortcut<EditorModeCommand>(new KeyGesture(Key.F5));
    }
}