using Gemini.Framework.Commands;

namespace Calame
{
    static public class CommandServiceExtension
    {
        static public TargetableCommand GetTargetableCommand<TCommandDefinition>(this ICommandService commandService)
            where TCommandDefinition : CommandDefinition
        {
            return commandService.GetTargetableCommand(commandService.GetCommand(commandService.GetCommandDefinition(typeof(TCommandDefinition))));
        }
    }
}