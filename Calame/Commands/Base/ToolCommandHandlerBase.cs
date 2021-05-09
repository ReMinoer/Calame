using System.Threading.Tasks;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands.Base
{
    public abstract class ToolCommandHandlerBase<TTool, TCommandDefinition> : CalameCommandHandlerBase<TCommandDefinition>
        where TTool : ITool
        where TCommandDefinition : CommandDefinition
    {
        protected readonly IShell Shell;
        protected TTool Tool;

        protected ToolCommandHandlerBase()
        {
            Shell = IoC.Get<IShell>();
        }

        protected override sealed void RefreshContext(Command command)
        {
            base.RefreshContext(command);

            if (Tool == null)
                Tool = Shell.Tools.OfType<TTool>().FirstOrDefault();

            RefreshContext(command, Tool);
        }

        protected override sealed bool CanRun(Command command)
        {
            return base.CanRun(command)
                && CanRun(command, Tool);
        }

        protected override sealed void UpdateStatus(Command command)
        {
            base.UpdateStatus(command);
            UpdateStatus(command, Tool);
        }

        protected override sealed Task RunAsync(Command command)
        {
            Shell.ShowTool(Tool);
            return RunAsync(command, Tool);
        }

        protected virtual void RefreshContext(Command command, TTool tool) { }
        protected virtual bool CanRun(Command command, TTool tool) => true;
        protected virtual void UpdateStatus(Command command, TTool tool) { }
        protected abstract Task RunAsync(Command command, TTool tool);
    }
}