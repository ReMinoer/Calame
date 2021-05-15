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
            protected override bool CanRun(LogConsoleViewModel tool)
            {
                return base.CanRun(tool)
                    && tool.CurrentDocumentLogEntries != null
                    && tool.CurrentDocumentLogEntries.Count > 0;
            }

            protected override void Run(LogConsoleViewModel tool)
            {
                tool.CurrentDocumentLogEntries?.Clear();
            }
        }
    }
}