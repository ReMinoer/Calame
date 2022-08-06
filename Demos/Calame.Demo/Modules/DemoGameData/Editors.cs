using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using Calame.DataModelViewer;
using Calame.DataModelViewer.Base;
using Calame.Demo.Data.Data;
using Calame.Demo.Data.Engine;
using Calame.Dialogs;
using Calame.Icons;
using Glyph;
using Glyph.Composition.Modelization;
using Glyph.Content;
using Glyph.IO;
using Glyph.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using Niddle;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(IEditorSource))]
    public class SceneEditorSource : DemoEditorSource<SceneData, SceneDemoEditor>
    {
        [ImportingConstructor]
        public SceneEditorSource(IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(importedTypeProvider, iconProvider, iconDescriptorManager, "Scene", ".scene") {}
    }

    [Export(typeof(IEditorSource))]
    public class RectangleEditorSource : DemoEditorSource<RectangleData, DemoEditor<RectangleData>>
    {
        [ImportingConstructor]
        public RectangleEditorSource(IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(importedTypeProvider, iconProvider, iconDescriptorManager, "Rectangle", ".rectangle") {}
    }

    [Export(typeof(IEditorSource))]
    public class CircleEditorSource : DemoEditorSource<CircleData, DemoEditor<CircleData>>
    {
        [ImportingConstructor]
        public CircleEditorSource(IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager)
            : base(importedTypeProvider, iconProvider, iconDescriptorManager, "Circle", ".circle") {}
    }

    public class DemoEditorSource<T, TEditor> : SerializingEditorSourceBase<T, TEditor>
        where T : IGlyphCreator, new()
        where TEditor : DemoEditor<T>, new()
    {
        protected override ISerializationFormat SerializationFormat { get; } = new DataContractSerializationFormat();
        protected readonly IIconProvider IconProvider;
        protected readonly IIconDescriptorManager IconDescriptorManager;

        public DemoEditorSource(IImportedTypeProvider importedTypeProvider, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager, string displayName, string extension)
            : base(importedTypeProvider, displayName, extension)
        {
            IconProvider = iconProvider;
            IconDescriptorManager = iconDescriptorManager;
        }

        public override TEditor CreateEditor()
        {
            TEditor editor = base.CreateEditor();
            editor.IconProvider = IconProvider;
            editor.IconDescriptorManager = IconDescriptorManager;
            return editor;
        }
    }

    public class DemoEditor<T> : SerializingViewerEditorBase<T>
        where T : IGlyphCreator, new()
    {
        protected RawContentLibrary ContentManager;
        protected string ContentRootPath;

        public IIconProvider IconProvider { get; set; }
        public IIconDescriptorManager IconDescriptorManager { get; set; }

        public override IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger)
        {
            ContentManager = EditorContentLibrary.GetOrCreateInstance(graphicsDeviceService, logger);
            ContentRootPath = PathUtils.NormalizeFolder(ContentManager.RawRootPath);
            return ContentManager;
        }

        protected override void RegisterDependencies(IDependencyRegistry registry)
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
                    string assetPath = PathUtils.CanBeRelative(filePath, ContentRootPath)
                        ? Path.ChangeExtension(PathUtils.MakeRelative(filePath, ContentRootPath), null)
                        : ImportAssetDialog.ShowDialog(filePath, ContentManager, IconProvider, IconDescriptorManager);

                    if (assetPath == null)
                        continue;

                    IAsset<object> asset = EditorContentLibrary.Instance.GetAsset<object>(assetPath);
                    asset.Handle();

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
                            Data.Instances.Add(new SpriteInstanceData
                            {
                                AssetPath = assetPath
                            });
                            break;
                        case null:
                            Data.Instances.Add(new FileInstanceData
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
        static public RawContentLibrary GetOrCreateInstance(IGraphicsDeviceService graphicsDeviceService, ILogger logger)
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

            Instance = new RawContentLibrary(graphicsDeviceService, logger, TargetPlatform.Windows, RawRootPath, cacheRootPath);
            return Instance;
        }
    }
}