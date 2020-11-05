using System;
using System.ComponentModel.Composition;
using Calame.Icons.Base;
using Diese;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Simulacra;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IGlyphData>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IGlyphData>))]
    public class DataBindedObjectIconDescriptor : TypeReTargetingDefaultDescriptorModuleBase<IGlyphData, IGlyphComponent>
    {
        [ImportingConstructor]
        public DataBindedObjectIconDescriptor([ImportMany] IIconDescriptorModule<IGlyphComponent>[] modules, [Import] IDefaultIconDescriptorModule<IGlyphComponent> defaultModule,
            [ImportMany] ITypeIconDescriptorModule<IGlyphComponent>[] typeModules, [Import] ITypeDefaultIconDescriptorModule<IGlyphComponent> typeDefaultModule)
            : base(modules, defaultModule, typeModules, typeDefaultModule) {}

        protected override IGlyphComponent GetTarget(IGlyphData model) => model?.BindedObject;
        protected override Type GetTypeTarget(Type type) => type.GetProperty(nameof(IBindableData.BindedObject))?.PropertyType;
    }
}