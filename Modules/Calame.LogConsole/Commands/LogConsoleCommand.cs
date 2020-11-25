using System.ComponentModel.Composition;
using Calame.Commands;
using Calame.Commands.Base;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.LogConsole.Commands
{
    [CommandHandler]
    public class LogConsoleCommand : OpenToolCommandBase<LogConsoleCommand.Definition, LogConsoleViewModel>
    {
        [CommandDefinition]
        public class Definition : CalameCommandDefinitionBase
        {
            public override string Name => "LogConsole.Open";
            public override string Text => "_Log Console";
        }

        [ImportingConstructor]
        public LogConsoleCommand(IShell shell)
            : base(shell)
        {
        }
    }
}