﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.SceneGraph.Commands;
using Calame.SceneGraph.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;

namespace Calame.SceneGraph
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<SceneGraphCommand>(MenuDefinitions.ViewToolsMenuGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(SceneGraphViewModel);
            }
        }
    }
}