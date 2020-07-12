using System.ComponentModel.Composition;
using System.Windows;
using Calame.DataModelViewer;
using Calame.DataModelViewer.Base;
using Calame.Demo.Data.Data;
using Calame.Demo.Data.Engine;
using Glyph.Composition.Modelization;
using Glyph.IO;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IEditorSource))]
    public class SceneEditorSource : DemoEditorSource<SceneData, SceneDemoEditor>
    {
        [ImportingConstructor]
        public SceneEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Scene", ".scene") {}
    }

    [Export(typeof(IEditorSource))]
    public class RectangleEditorSource : DemoEditorSource<RectangleData, DemoEditor<RectangleData>>
    {
        [ImportingConstructor]
        public RectangleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Rectangle", ".rectangle") {}
    }

    [Export(typeof(IEditorSource))]
    public class CircleEditorSource : DemoEditorSource<CircleData, DemoEditor<CircleData>>
    {
        [ImportingConstructor]
        public CircleEditorSource(IImportedTypeProvider importedTypeProvider)
            : base(importedTypeProvider, "Circle", ".circle") {}
    }

    public class DemoEditorSource<T, TEditor> : SerializingEditorSourceBase<T, TEditor>
        where T : IGlyphCreator, new()
        where TEditor : SerializingViewerEditorBase<T>, new()
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
            registry.Add(Dependency.OnType<InstanceObject>());
            registry.Add(Dependency.OnType<RectangleObject>());
            registry.Add(Dependency.OnType<CircleObject>());
        }
    }

    public class SceneDemoEditor : DemoEditor<SceneData>
    {
        public override void OnDragOver(DragEventArgs dragEventArgs)
        {
            if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                dragEventArgs.Effects = DragDropEffects.Link;
                dragEventArgs.Handled = true;
                return;
            }

            base.OnDragOver(dragEventArgs);
        }

        public override void OnDrop(DragEventArgs dragEventArgs)
        {
            var filePaths = dragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (filePaths != null && filePaths.Length != 0)
            {
                foreach (string filePath in filePaths)
                {
                    Creator.Instances.Add(new InstanceData
                    {
                        FilePath = filePath
                    });
                }
                return;
            }

            base.OnDrop(dragEventArgs);
        }
    }
}