using System.ComponentModel.Composition;
using Calame.CompositionGraph.Commands;
using Gemini.Framework.Menus;

namespace Calame.CompositionGraph
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition CompositionGraphCommandMenuItem = new CommandMenuItemDefinition<CompositionGraphCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}