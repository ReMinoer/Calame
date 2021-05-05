using System.Threading.Tasks;
using Calame.Commands.Base;
using Calame.Icons;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;

namespace Calame.LogConsole.Commands
{
    [CommandDefinition]
    public class AutoScrollLogCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Auto Scroll Log";
        public override object IconKey => CalameIconKey.AutoScroll;

        [CommandHandler]
        public class CommandHandler : ToolCommandHandlerBase<LogConsoleViewModel, AutoScrollLogCommand>
        {
            protected override void UpdateStatus(Command command, LogConsoleViewModel tool)
            {
                base.UpdateStatus(command, tool);
                command.Checked = tool.AutoScroll;
            }

            protected override Task RunAsync(Command command, LogConsoleViewModel tool)
            {
                tool.AutoScroll = !tool.AutoScroll;
                return Task.CompletedTask;
            }
        }
    }
}