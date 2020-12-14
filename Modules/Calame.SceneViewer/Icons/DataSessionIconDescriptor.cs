using System;
using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;
using Glyph.Composition.Modelization;

namespace Calame.SceneViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<ISession>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<ISession>))]
    public class DataSessionIconDescriptor : TypeReTargetingDefaultDescriptorModuleBase<ISession, IGlyphData>
    {
        protected override IGlyphData GetTarget(ISession model) => (model as IDataSession)?.Data;
        protected override Type GetTypeTarget(Type type) => type?.GetProperty(nameof(IDataSession.Data))?.PropertyType;

        protected override IconDescription TransformIcon(IconDescription iconDescription) => new IconDescription(iconDescription.Key, IconBrushes.Default);
        protected override IconDescription GetNotTargetedTypeDefaultIcon(Type type) => new IconDescription(CalameIconKey.SessionMode, IconBrushes.Default);
    }
}