﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Calame.DataModelTree.ViewModels;
using Gemini.Framework;
using Simulacra.IO.Binding;

namespace Calame.DataModelTree
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
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
            base.PreInitialize();
        }
    }
}