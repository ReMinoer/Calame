using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Calame.Icons.Base;
using Diese;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    public class SystemTypesIconDescriptor : TypeDefaultIconDescriptorModuleBase
    {
        static public readonly Brush DefaultBrush = IconBrushes.Default;

        public override bool Handle(Type type) => type?.Namespace?.StartsWith(nameof(System)) ?? false;

        public override IconDescription GetDefaultIcon(object model)
        {
            if (model is bool value)
                return new IconDescription(value ? PackIconMaterialKind.AlphaTCircle : PackIconMaterialKind.AlphaFCircleOutline, DefaultBrush);

            return base.GetDefaultIcon(model);
        }

        public override IconDescription GetTypeDefaultIcon(Type type)
        {
            if (type.Is<bool>())
                return new IconDescription(PackIconMaterialKind.AlphaBCircleOutline, DefaultBrush);
            if (type.Is<int>()
                || type.Is<float>()
                || type.Is<double>()
                || type.Is<long>()
                || type.Is<short>()
                || type.Is<byte>()
                || type.Is<uint>()
                || type.Is<ulong>()
                || type.Is<ushort>()
                || type.Is<sbyte>()
                || type.Is<char>()
                || type.Is<decimal>()
                || type.Is<Enum>())
                return new IconDescription(PackIconMaterialKind.Pound, DefaultBrush);
            if (type.Is<string>()
                || type.Is<Uri>())
                return new IconDescription(PackIconMaterialKind.CodeString, DefaultBrush);
            if (type.Is<Array>())
                return new IconDescription(PackIconMaterialKind.CodeArray, DefaultBrush);
            if (type.Is<Type>())
                return new IconDescription(PackIconMaterialKind.CodeNotEqualVariant, DefaultBrush);
            if (type.Is<Delegate>())
                return new IconDescription(PackIconMaterialKind.CodeBracesBox, DefaultBrush);
            if (type.Is<ValueTuple>())
                return new IconDescription(PackIconMaterialKind.SelectGroup, DefaultBrush);
            if (type.Is<DateTime>())
                return new IconDescription(PackIconMaterialKind.CalendarClock, DefaultBrush);
            if (type.Is<TimeSpan>())
                return new IconDescription(PackIconMaterialKind.Clock, DefaultBrush);
            if (type.Is<Guid>())
                return new IconDescription(PackIconMaterialKind.Identifier, DefaultBrush);
            if (type.Is<IntPtr>()
                || type.Is<UIntPtr>())
                return new IconDescription(PackIconMaterialKind.TableArrowUp, DefaultBrush);

            return IconDescription.None;
        }
    }
}