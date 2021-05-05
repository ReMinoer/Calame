using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.LogConsole.Commands
{
    [CommandDefinition]
    public class ClearLogCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Clear Log";
        public override object IconKey => CalameIconKey.Clear;

        [CommandHandler]
        public class CommandHandler : ToolCommandHandlerBase<LogConsoleViewModel, ClearLogCommand>
        {
            protected override bool CanRun(Command command, LogConsoleViewModel tool)
            {
                return base.CanRun(command, tool)
                    && tool.CurrentDocumentLogEntries != null
                    && tool.CurrentDocumentLogEntries.Count > 0;
            }

            protected override Task RunAsync(Command command, LogConsoleViewModel tool)
            {
                tool.CurrentDocumentLogEntries?.Clear();
                return Task.CompletedTask;
            }
        }
    }
}