using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.LogConsole.Commands;
using Calame.LogConsole.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;

namespace Calame.LogConsole
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<LogConsoleCommand>(MenuDefinitions.ViewToolsMenuGroup, 3);

        public Module()
        {
            NLog.LogManager.CreateNullLogger();
        }
        
        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(LogConsoleViewModel);
            }
        }
    }
}