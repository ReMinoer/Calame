using System.ComponentModel.Composition;
using Calame.DataModelViewer;
using Calame.DataModelViewer.Base;
using Calame.Demo.Data;
using Glyph.Composition.Modelization;
using Glyph.IO;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditorSource))]
    public class SceneEditorSource : DemoEditorSource<SceneData>
    {
        [ImportingConstructor]
        public SceneEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Scene", ".scene") {}
    }

    [Export(typeof(IEditorSource))]
    public class RectangleEditorSource : DemoEditorSource<RectangleData>
    {
        [ImportingConstructor]
        public RectangleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Rectangle", ".rectangle") {}
    }

    [Export(typeof(IEditorSource))]
    public class CircleEditorSource : DemoEditorSource<CircleData>
    {
        [ImportingConstructor]
        public CircleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Circle", ".circle") {}
    }

    public class DemoEditorSource<T> : SerializingEditorSourceBase<T, DemoEditor<T>>
        where T : IGlyphCreator, new()
    {
        protected override ISerializationFormat SerializationFormat { get; } = new DataContractSerializationFormat();

        public DemoEditorSource(IImportedTypeProvider importedTypeProvider, string displayName, string extension)
            : base(importedTypeProvider, displayName, extension)
        {
        }
    }
    
    public class DemoEditor<T> : SerializingViewerEditorBase<T>
        where T : IGlyphCreator, new()
    {
        public override string ContentPath => null;

        public override void RegisterDependencies(IDependencyRegistry registry)
        {
            base.RegisterDependencies(registry);
            registry.Add(Dependency.OnType<Scene>());
            registry.Add(Dependency.OnType<RectangleObject>());
            registry.Add(Dependency.OnType<CircleObject>());
        }
    }
}