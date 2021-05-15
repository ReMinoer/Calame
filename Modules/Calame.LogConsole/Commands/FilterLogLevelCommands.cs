using Calame.Commands.Base;
using Calame.LogConsole.ViewModels;
using Gemini.Framework.Commands;
using Microsoft.Extensions.Logging;

namespace Calame.LogConsole.Commands
{
    [CommandDefinition]
    public class FilterLogTraceCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Trace;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogTraceCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Trace;
        }
    }

    [CommandDefinition]
    public class FilterLogDebugCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Debug;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogDebugCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Debug;
        }
    }

    [CommandDefinition]
    public class FilterLogInformationCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Information;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogInformationCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Information;
        }
    }

    [CommandDefinition]
    public class FilterLogWarningCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Warning;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogWarningCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Warning;
        }
    }

    [CommandDefinition]
    public class FilterLogErrorCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Error;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogErrorCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Error;
        }
    }

    [CommandDefinition]
    public class FilterLogCriticalCommand : FilterLogLevelCommandDefinitionBase
    {
        protected override LogLevel LogLevel => LogLevel.Critical;

        [CommandHandler]
        public class CommandHandler : FilterLogLevelCommandHandlerBase<FilterLogCriticalCommand>
        {
            protected override LogLevel LogLevel => LogLevel.Critical;
        }
    }

    public abstract class FilterLogLevelCommandDefinitionBase : CalameCommandDefinitionBase
    {
        public override string Text => LogLevel.ToString();
        public override object IconKey => LogLevel;

        protected abstract LogLevel LogLevel { get; }
    }

    public abstract class FilterLogLevelCommandHandlerBase<TDefinition> : ToolCommandHandlerBase<LogConsoleViewModel, TDefinition>
        where TDefinition : CommandDefinition
    {
        protected abstract LogLevel LogLevel { get; }

        protected override void UpdateStatus(Command command, LogConsoleViewModel tool)
        {
            base.UpdateStatus(command, tool);
            command.Checked = !tool.HiddenLogLevels.Contains(LogLevel);
        }

        protected override void Run(LogConsoleViewModel tool)
        {
            if (tool.HiddenLogLevels.Contains(LogLevel))
                tool.HiddenLogLevels.Remove(LogLevel);
            else
                tool.HiddenLogLevels.Add(LogLevel);
        }
    }
}