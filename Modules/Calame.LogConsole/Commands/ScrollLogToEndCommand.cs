using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.LogConsole.Commands
{
    [CommandDefinition]
    public class ScrollLogToEndCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Scroll Log To End";
        public override object IconKey => CalameIconKey.ScrollToEnd;

        [CommandHandler]
        public class CommandHandler : ToolCommandHandlerBase<LogConsoleViewModel, ScrollLogToEndCommand>
        {
            protected override bool CanRun(Command command, LogConsoleViewModel tool)
            {
                return base.CanRun(command, tool)
                    && tool.CurrentDocumentLogEntries != null
                    && tool.CurrentDocumentLogEntries.Count > 0;
            }

            protected override Task RunAsync(Command command, LogConsoleViewModel tool)
            {
                tool.ScrollToEnd();
                return Task.CompletedTask;
            }
        }
    }
}