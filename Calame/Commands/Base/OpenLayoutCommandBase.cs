using System.Threading.Tasks;
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

        protected override Task RunAsync(Command command)
        {
            foreach (ITool tool in _shell.Tools)
                tool.IsVisible = false;

            OpenLayout(_shell);
            return Task.CompletedTask;
        }

        protected abstract void OpenLayout(IShell shell);
    }
}