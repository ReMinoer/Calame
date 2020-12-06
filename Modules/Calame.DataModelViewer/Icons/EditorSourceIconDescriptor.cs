using System;
using System.ComponentModel.Composition;
using Calame.Icons;
using Calame.Icons.Base;

namespace Calame.DataModelViewer.Icons
{
    [Export(typeof(IDefaultIconDescriptorModule))]
    [Export(typeof(IDefaultIconDescriptorModule<IEditorSource>))]
    [Export(typeof(ITypeDefaultIconDescriptorModule))]
    [Export(typeof(ITypeDefaultIconDescriptorModule<IEditorSource>))]
    public class EditorSourceIconDescriptor : TypeReTargetingDefaultDescriptorModuleBase<IEditorSource, IEditor>
    {
        protected override IEditor GetTarget(IEditorSource model) => null;
        protected override Type GetTypeTarget(Type type) => type?.GetMethod(nameof(IEditorSource.CreateEditor))?.ReturnType;
    }
}