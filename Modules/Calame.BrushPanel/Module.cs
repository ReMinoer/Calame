using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Calame.BrushPanel.Commands;
using Calame.BrushPanel.ViewModels;
using Calame.Viewer;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;

namespace Calame.BrushPanel
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public readonly MenuItemDefinition MenuItem = new CommandMenuItemDefinition<BrushPanelCommand>(CalameMenus.WindowToolsGroup, 3);

        [Export]
        static public readonly ToolBarItemDefinition ViewerToolBarItem = new CommandToolBarItemDefinition<BrushModeCommand>(ViewerToolBar.ModesGroup, 10);
        [Export]
        static public readonly MenuItemDefinition ViewerMenuItem = new CommandMenuItemDefinition<BrushModeCommand>(ViewerMenu.ModesGroup, 10);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(BrushPanelViewModel);
            }
        }
    }

    static public class SessionShortcuts
    {
        [Export]
        static public readonly CommandKeyboardShortcut SessionMode = new CommandKeyboardShortcut<BrushModeCommand>(new KeyGesture(Key.F2, ModifierKeys.Control));
    }
}