using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands.Base
{
    public abstract class OpenToolCommandBase<TDefinition, TTool> : CalameCommandHandlerBase<TDefinition>
        where TDefinition : CommandDefinition
        where TTool : ITool
    {
        private readonly IShell _shell;

        public OpenToolCommandBase()
        {
            _shell = IoC.Get<IShell>();
        }

        protected override Task RunAsync(Command command)
        {
            _shell.ShowTool<TTool>();
            return Task.CompletedTask;
        }
    }
}