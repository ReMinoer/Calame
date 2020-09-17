using System.Threading.Tasks;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.Commands
{
    public abstract class OpenToolCommandBase<TDefinition, TTool> : CommandHandlerBase<TDefinition>
        where TDefinition : CommandDefinition
        where TTool : ITool
    {
        private readonly IShell _shell;

        public OpenToolCommandBase(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<TTool>();
            return TaskUtility.Completed;
        }
    }
}