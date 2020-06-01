using System.ComponentModel.Composition;
using Calame.Icons.Base;
using Glyph.Composition;
using Glyph.Composition.Modelization;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IIconDescriptorModule<IGlyphData>))]
    [Export(typeof(IDefaultIconDescriptorModule<IGlyphData>))]
    public class DataIconDescriptor : ReTargetingDefaultDescriptorModuleBase<IGlyphData, IGlyphComponent>
    {
        [ImportingConstructor]
        public DataIconDescriptor([ImportMany] IIconDescriptorModule<IGlyphComponent>[] modules, [Import] IDefaultIconDescriptorModule<IGlyphComponent> defaultModule)
            : base(modules, defaultModule) {}

        protected override IGlyphComponent GetTarget(IGlyphData model) => model?.BindedObject;
    }
}