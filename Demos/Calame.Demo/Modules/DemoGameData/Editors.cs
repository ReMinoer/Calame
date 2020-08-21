using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Calame.DataModelViewer;
using Calame.DataModelViewer.Base;
using Calame.Demo.Data.Data;
using Calame.Demo.Data.Engine;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Content;
using Glyph.IO;
using Glyph.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
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
        public override IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService)
        {
            return EditorContentLibrary.GetOrCreateInstance(graphicsDeviceService);
        }

        public override void RegisterDependencies(IDependencyRegistry registry)
        {
            registry.Add(Dependency.OnType<Scene>());
            registry.Add(Dependency.OnType<InstanceObject>());
            registry.Add(Dependency.OnType<SpriteInstanceObject>());
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

        public override async void OnDrop(DragEventArgs dragEventArgs)
        {
            var filePaths = dragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (filePaths != null && filePaths.Length != 0)
            {
                foreach (string filePath in filePaths)
                {
                    string assetName = Path.GetFileNameWithoutExtension(filePath);
                    string fileName = Path.GetFileName(filePath);
                    string copyFilePath = Path.Combine(EditorContentLibrary.RawRootPath, fileName);

                    await Task.Run(() => File.Copy(filePath, copyFilePath));

                    IAsset<object> asset = EditorContentLibrary.Instance.GetAsset<object>(assetName);

                    object content;
                    try
                    {
                        content = await asset.GetContentAsync();
                    }
                    catch (NoImporterException)
                    {
                        content = null;
                    }
                    finally
                    {
                        await asset.ReleaseAsync();
                    }

                    switch (content)
                    {
                        case Texture2D _:
                            Creator.Instances.Add(new SpriteInstanceData
                            {
                                AssetPath = assetName
                            });
                            break;
                        case null:
                            Creator.Instances.Add(new FileInstanceData
                            {
                                FilePath = filePath
                            });
                            break;
                    }

                }
                return;
            }

            base.OnDrop(dragEventArgs);
        }
    }

    static public class EditorContentLibrary
    {
        static public string RawRootPath;

        static public RawContentLibrary Instance { get; private set; }
        static public RawContentLibrary GetOrCreateInstance(IGraphicsDeviceService graphicsDeviceService)
        {
            if (Instance != null)
                return Instance;

            string contentFolder = Path.Combine(Path.GetTempPath(), "CalameDemoContent");

            RawRootPath = Path.Combine(contentFolder, "raw");
            string cacheRootPath = Path.Combine(contentFolder, "cooked");

            CreateFolder(contentFolder);
            CreateFolder(RawRootPath);
            CreateFolder(cacheRootPath);

            void CreateFolder(string folderPath)
            {
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, recursive: true);
                Directory.CreateDirectory(folderPath);
            }

            Instance = new RawContentLibrary(graphicsDeviceService, RawRootPath, cacheRootPath);
            return Instance;
        }
    }
}