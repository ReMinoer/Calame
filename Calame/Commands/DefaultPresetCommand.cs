using System;
using Calame.Commands.Base;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;

namespace Calame.Commands
{
    [CommandDefinition]
    public class DefaultPresetCommand : CalameCommandDefinitionBase
    {
        public override string Text => "_Default Preset";
        public override object IconKey => CalameIconKey.DefaultPreset;

        [CommandHandler]
        public class CommandHandler : OpenLayoutCommandBase<DefaultPresetCommand>
        {
            protected override void OpenLayout(IShell shell)
            {
                foreach (IModule module in IoC.GetAll<IModule>())
                    foreach (Type defaultToolType in module.DefaultTools)
                        shell.ShowTool((ITool)IoC.GetInstance(defaultToolType, null));
            }
        }
    }
}