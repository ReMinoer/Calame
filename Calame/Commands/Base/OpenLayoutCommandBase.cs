using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands.Base
{
    public abstract class OpenLayoutCommandBase<TDefinition> : CalameCommandHandlerBase<TDefinition>
        where TDefinition : CommandDefinition
    {
        private readonly IShell _shell;

        public OpenLayoutCommandBase()
        {
            _shell = IoC.Get<IShell>();
        }

        protected override void Run()
        {
            foreach (ITool tool in _shell.Tools)
                tool.IsVisible = false;

            OpenLayout(_shell);
        }

        protected abstract void OpenLayout(IShell shell);
    }
}