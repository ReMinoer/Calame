using System;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Calame.Commands.Base
{
    public abstract class CalameCommandDefinitionBase : CommandDefinition
    {
        public override string ToolTip => Text.Replace("_", "");
        public override sealed string Name { get; }
        public override sealed Uri IconSource { get; }
        public abstract object IconKey { get; }

        protected CalameCommandDefinitionBase()
        {
            Name = GetType().FullName;

            if (IconKey != null)
            {
                IconDescription iconDescription = IoC.Get<IIconDescriptorManager>().GetDescriptor().GetIcon(IconKey);
                IconSource = IoC.Get<IIconProvider>().GetUri(iconDescription, 16);
            }
        }
    }
}