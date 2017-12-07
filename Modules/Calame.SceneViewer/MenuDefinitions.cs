using System.ComponentModel.Composition;
using Gemini.Framework.Menus;

namespace Calame.SceneViewer
{
    static public class MenuDefinitions
    {
        [Export]
        static public MenuDefinition SessionsMenu = new MenuDefinition(Gemini.Modules.MainMenu.MenuDefinitions.MainMenuBar, 5, "_Sessions");
    }
}