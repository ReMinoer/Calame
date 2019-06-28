using System.ComponentModel.Composition;
using Calame.BrushPanel.Commands;
using Gemini.Framework.Menus;

namespace Calame.BrushPanel
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition BrushPanelCommandMenuItem = new CommandMenuItemDefinition<BrushPanelCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}