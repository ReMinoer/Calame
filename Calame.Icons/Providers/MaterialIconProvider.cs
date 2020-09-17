using System.ComponentModel.Composition;
using Calame.Icons.Providers.Base;
using MahApps.Metro.IconPacks;

namespace Calame.Icons.Providers
{
    [Export(typeof(IIconProviderModule))]
    public class MaterialIconProvider : MahAppsIconProviderModuleBase<PackIconMaterialKind, PackIconMaterial>
    {
        protected override void AssignKindToControl(PackIconMaterial control, PackIconMaterialKind kind) => control.Kind = kind;
        protected override string GetGeometryPath(PackIconMaterialKind iconKind) => PackIconMaterialDataFactory.DataIndex.Value[iconKind];
    }
}