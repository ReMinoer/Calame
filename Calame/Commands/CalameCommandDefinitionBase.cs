using System;
using Calame.Icons;
using Caliburn.Micro;
using Gemini.Framework.Commands;

namespace Calame.Commands
{
    public abstract class CalameCommandDefinitionBase : CommandDefinition, ICommandDefinition
    {
        public override string ToolTip => Text;

        private Uri _iconSource;
        public override Uri IconSource
        {
            get
            {
                if (_iconSource == null)
                {
                    var iconProvider = IoC.Get<IIconProvider>();
                    var iconDescriptorManager = IoC.Get<IIconDescriptorManager>();

                    IIconDescriptor iconDescriptor = iconDescriptorManager.GetDescriptor();
                    IconDescription iconDescription = iconDescriptor.GetIcon(this);

                    _iconSource = iconProvider.GetUri(iconDescription, 16);
                }

                return _iconSource;
            }
        }
    }
}