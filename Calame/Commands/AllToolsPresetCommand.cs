using Calame.Commands.Base;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands
{
    [CommandDefinition]
    public class AllToolsPresetCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_All Tools";
        public override object IconKey => null;

        [CommandHandler]
        public class CommandHandler : OpenLayoutCommandBase<AllToolsPresetCommand>
        {
            protected override void OpenLayout(IShell shell)
            {
                foreach (ITool tool in shell.Tools)
                    tool.IsVisible = true;
            }
        }
    }
}