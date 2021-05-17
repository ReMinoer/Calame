using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Services;
using Gemini.Modules.Shell.ViewModels;

namespace Calame.Commands.Base
{
    public abstract class LayoutCommandHandlerBase<TDefinition> : CalameCommandHandlerBase<TDefinition>
        where TDefinition : CommandDefinition
    {
        private readonly ILayoutItemStatePersister _layoutItemStatePersister;
        private readonly ShellViewModel _shellViewModel;

        protected LayoutCommandHandlerBase()
        {
            _shellViewModel = IoC.Get<IShell>() as ShellViewModel;
            _layoutItemStatePersister = IoC.Get<ILayoutItemStatePersister>();
        }

        protected override bool CanRun()
        {
            return base.CanRun()
                && _layoutItemStatePersister != null
                && _shellViewModel != null;
        }

        protected override void Run()
        {
            Run(_layoutItemStatePersister, _shellViewModel);
        }

        protected abstract void Run(ILayoutItemStatePersister layoutItemStatePersister, ShellViewModel shellViewModel);
    }
}