using System;
using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;
using Glyph.Composition.Modelization;

namespace Calame.DataModelViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IEditor>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IEditor>))]
    public class EditorIconDescriptor : TypeReTargetingDefaultDescriptorModuleBase<IEditor, IGlyphData>
    {
        protected override IGlyphData GetTarget(IEditor model) => model?.Data;
        protected override Type GetTypeTarget(Type type) => type?.GetProperty(nameof(IEditor.Data))?.PropertyType;
        protected override IconDescription TransformIcon(IconDescription iconDescription) => new IconDescription(iconDescription.Key, IconBrushes.Default);
    }
}