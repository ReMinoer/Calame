using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace Calame.Commands.Base
{
    public abstract class CalameCommandHandlerBase<TCommandDefinition> : CommandHandlerBase<TCommandDefinition>
        where TCommandDefinition : CommandDefinition
    {
        public override sealed void Update(Command command)
        {
            RefreshContext(command);
            command.Enabled = CanRun();
            UpdateStatus(command);
        }

        public override sealed Task Run(Command command)
        {
            // CommandHandlerBase is using async void to run a task.
            // We will run it synchronously to prevent simultaneous accesses and not caught exceptions.
            if (CanRun())
                Run();

            return Task.CompletedTask;
        }

        protected virtual void RefreshContext(Command command) { }
        protected virtual bool CanRun() => true;
        protected virtual void UpdateStatus(Command command) { }
        protected abstract void Run();
    }
}