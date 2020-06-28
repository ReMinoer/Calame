using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    public class BaseTypesIconDescriptor : IDefaultIconDescriptorModule
    {
        static public readonly Brush DefaultBrush = Brushes.Black;

        public IconDescription GetDefaultIcon(object model)
        {
            switch (model)
            {
                case bool value:
                    return new IconDescription(value ? PackIconMaterialKind.AlphaTCircle : PackIconMaterialKind.AlphaFCircleOutline, DefaultBrush);
                case int _:
                case float _:
                case double _:
                case long _:
                case short _:
                case byte _:
                case uint _:
                case ulong _:
                case ushort _:
                case sbyte _:
                case char _:
                case decimal _:
                case Enum _:
                    return new IconDescription(PackIconMaterialKind.Pound, DefaultBrush);
                case string _:
                case Uri _:
                    return new IconDescription(PackIconMaterialKind.CodeString, DefaultBrush);
                case Array _:
                    return new IconDescription(PackIconMaterialKind.CodeArray, DefaultBrush);
                case Type _:
                    return new IconDescription(PackIconMaterialKind.CodeNotEqualVariant, DefaultBrush);
                case Delegate _:
                    return new IconDescription(PackIconMaterialKind.CodeBracesBox, DefaultBrush);
                case ValueTuple _:
                    return new IconDescription(PackIconMaterialKind.SelectGroup, DefaultBrush);
                case DateTime _:
                    return new IconDescription(PackIconMaterialKind.CalendarClock, DefaultBrush);
                case TimeSpan _:
                    return new IconDescription(PackIconMaterialKind.Clock, DefaultBrush);
                case Guid _:
                    return new IconDescription(PackIconMaterialKind.Identifier, DefaultBrush);
                case IntPtr _:
                case UIntPtr _:
                    return new IconDescription(PackIconMaterialKind.TableArrowUp, DefaultBrush);
                default:
                    return IconDescription.None;
            }
        }
    }
}