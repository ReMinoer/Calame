using System.ComponentModel.Composition;
using Calame.Icons.Base;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IGlyphData>))]
    public class DataBindedObjectIconDescriptor : ReTargetingDefaultDescriptorModuleBase<IGlyphData, IGlyphComponent>
    {
        [ImportingConstructor]
        public DataBindedObjectIconDescriptor([ImportMany] IIconDescriptorModule<IGlyphComponent>[] modules, [Import] IDefaultIconDescriptorModule<IGlyphComponent> defaultModule)
            : base(modules, defaultModule) {}

        protected override IGlyphComponent GetTarget(IGlyphData model) => model?.BindedObject;
    }
}