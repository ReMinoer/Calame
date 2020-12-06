using System;
using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;
using Glyph.Composition.Modelization;

namespace Calame.SceneViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IDataSession>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IDataSession>))]
    public class DataSessionIconDescriptor : TypeReTargetingDefaultDescriptorModuleBase<IDataSession, IGlyphData>
    {
        protected override IGlyphData GetTarget(IDataSession model) => model?.Data;
        protected override Type GetTypeTarget(Type type) => type?.GetProperty(nameof(IDataSession.Data))?.PropertyType;
        protected override IconDescription TransformIcon(IconDescription iconDescription) => new IconDescription(iconDescription.Key, IconBrushes.Default);
    }
}