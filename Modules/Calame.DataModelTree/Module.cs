using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Calame.DataModelTree.Commands;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Microsoft.Extensions.Logging;
using Simulacra.IO.Binding;

namespace Calame.DataModelTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Export]
        static public readonly MenuItemDefinition MenuItem = new CommandMenuItemDefinition<DataModelTreeCommand>(CalameMenus.WindowToolsGroup, 3);

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(DataModelTreeViewModel);
            }
        }

        private readonly ILoggerProvider _loggerProvider;

        [ImportingConstructor]
        public Module(ILoggerProvider loggerProvider)
        {
            _loggerProvider = loggerProvider;
        }

        public override void PreInitialize()
        {
            PathBindingModule.DefaultSynchronizationContext = SynchronizationContext.Current;
            PathBindingModule.Watcher.Logger = _loggerProvider.CreateLogger(nameof(PathBindingModule));
            base.PreInitialize();
        }
    }
}