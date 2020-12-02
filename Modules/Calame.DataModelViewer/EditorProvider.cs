using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Calame.DataModelViewer.ViewModels;
using Calame.Icons;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.DataModelViewer
{
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly CompositionContainer _compositionContainer;
        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor<IEditorSource> _iconDescriptor;

        public List<IEditorSource> Editors { get; } = new List<IEditorSource>();
        public IEnumerable<EditorFileType> FileTypes => Editors.SelectMany(x => x.FileExtensions.Select(e => new EditorFileType(x.DisplayName, e, _iconProvider.GetUri(_iconDescriptor.GetIcon(x), 16))));

        [ImportingConstructor]
        public EditorProvider(CompositionContainer compositionContainer, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager,
            [ImportMany] IEnumerable<IEditorSource> editors)
        {
            _compositionContainer = compositionContainer;
            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor<IEditorSource>();

            if (editors != null)
                Editors.AddRange(editors);
        }

        public bool Handles(string path)
        {
            return Editors.Any(x => Handles(x, path));
        }

        static private bool Handles(IEditorSource editor, string path)
        {
            return editor.FileExtensions.Contains(Path.GetExtension(path));
        }

        public IDocument Create()
        {
            return _compositionContainer.GetExportedValue<DataModelViewerViewModel>();
        }

        public async Task New(IDocument document, string name)
        {
            var dataModelViewerViewModel = (DataModelViewerViewModel)document;
            dataModelViewerViewModel.Editor = Editors.First(x => Handles(x, name)).CreateEditor();
            await dataModelViewerViewModel.New(name);
        }

        public async Task Open(IDocument document, string path)
        {
            var dataModelViewerViewModel = (DataModelViewerViewModel)document;
            dataModelViewerViewModel.Editor = Editors.First(x => Handles(x, path)).CreateEditor();
            await dataModelViewerViewModel.Load(path);
        }
    }
}