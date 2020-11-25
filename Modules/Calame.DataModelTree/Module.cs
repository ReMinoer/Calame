using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Calame.DataModelTree.Commands;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Modules.MainMenu;
using NLog.Extensions.Logging;
using Simulacra.IO.Binding;

namespace Calame.DataModelTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public MenuItemDefinition MenuItem = new CommandMenuItemDefinition<DataModelTreeCommand>(MenuDefinitions.ViewToolsMenuGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(DataModelTreeViewModel);
            }
        }

        public override void PreInitialize()
        {
            PathBindingModule.DefaultSynchronizationContext = SynchronizationContext.Current;
            PathBindingModule.Watcher.Logger = new NLogLoggerProvider().CreateLogger(GetType().FullName);
            base.PreInitialize();
        }
    }
}