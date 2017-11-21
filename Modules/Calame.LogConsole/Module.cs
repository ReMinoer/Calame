using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Calame.LogConsole.ViewModels;
using Gemini.Framework;

namespace Calame.LogConsole
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
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