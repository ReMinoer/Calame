using System;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Calame.Commands.Base
{
    public abstract class CalameCommandDefinitionBase : CommandDefinition
    {
        public override string ToolTip => Text;
        public override string Name { get; }
        public override sealed Uri IconSource { get; }

        protected CalameCommandDefinitionBase()
        {
            Name = GetType().FullName;

            IconDescription iconDescription = IoC.Get<IIconDescriptorManager>().GetDescriptor<CommandDefinition>().GetIcon(this);
            IconSource = IoC.Get<IIconProvider>().GetUri(iconDescription, 16);
        }
    }
}