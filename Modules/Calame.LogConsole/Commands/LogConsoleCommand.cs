using Calame.Commands;
using Calame.Commands.Base;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.LogConsole.Commands
{
    [CommandDefinition]
    public class LogConsoleCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Log Console";

        [CommandHandler]
        public class CommandHandler : OpenToolCommandBase<LogConsoleCommand, LogConsoleViewModel>
        {
        }
    }
}