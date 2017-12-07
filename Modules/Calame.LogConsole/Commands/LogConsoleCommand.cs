using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace Calame.LogConsole.Commands
{
    [CommandHandler]
    public class LogConsoleCommand : CommandHandlerBase<LogConsoleCommand.Definition>
    {
        [CommandDefinition]
        public class Definition : CommandDefinition
        {
            public const string CommandName = "LogConsole.Open";
            public override string Name => CommandName;
            public override string Text => "_Log Console";
            public override string ToolTip => "_Log Console";
        }

        private readonly IShell _shell;

        [ImportingConstructor]
        public LogConsoleCommand(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<LogConsoleViewModel>();
            return TaskUtility.Completed;
        }
    }
}