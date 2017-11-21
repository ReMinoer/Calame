using System.ComponentModel.Composition;
using Calame.SceneGraph.Commands;
using Gemini.Framework.Menus;

namespace Calame.SceneGraph
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuItemDefinition SceneGraphCommandMenuItem = new CommandMenuItemDefinition<SceneGraphCommand.Definition>(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 3);
    }
}