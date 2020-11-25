using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Gemini.Framework.Commands;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<Command>))]
    public class DynamicCommandIconDescriptor : IconDescriptorModuleBase<Command>
    {
        public override IconDescription GetIcon(Command command)
        {
            switch (command.CommandDefinition.GetType().Name)
            {
                case "EnginePauseResumeCommand":
                    if ((bool)command.Tag)
                        return new IconDescription(PackIconMaterialKind.Play, Brushes.Green);
                    else
                        return new IconDescription(PackIconMaterialKind.Pause, Brushes.RoyalBlue);
                default:
                    return IconDescription.None;
            }
        }
    }
}