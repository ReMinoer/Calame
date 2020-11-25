using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace Calame.Commands.Base
{
    public abstract class CalameCommandHandlerBase<TCommandDefinition> : CommandHandlerBase<TCommandDefinition>
        where TCommandDefinition : CommandDefinition
    {
        public override sealed void Update(Command command)
        {
            command.Enabled = CanRun(command);
            base.Update(command);
        }

        public override sealed async Task Run(Command command)
        {
            if (CanRun(command))
                await RunAsync(command);
        }

        protected virtual bool CanRun(Command command) => true;
        protected abstract Task RunAsync(Command command);
    }
}