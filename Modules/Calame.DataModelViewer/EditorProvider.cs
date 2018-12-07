using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Calame.DataModelViewer.ViewModels;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.DataModelViewer
{
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly IContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;
        private readonly IImportedTypeProvider _importedTypeProvider;
        private readonly IShell _shell;
        public List<IEditor> Editors { get; } = new List<IEditor>();
        public IEnumerable<EditorFileType> FileTypes => Editors.SelectMany(x => x.FileExtensions.Select(e => new EditorFileType(x.DisplayName, e)));

        [ImportingConstructor]
        public EditorProvider(IContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator, IImportedTypeProvider importedTypeProvider, [ImportMany] IEnumerable<IEditor> editors = null)
        {
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;
            _importedTypeProvider = importedTypeProvider;

            if (editors != null)
                Editors.AddRange(editors);
        }

        public bool Handles(string path)
        {
            return Editors.SelectMany(x => x.FileExtensions).Contains(Path.GetExtension(path));
        }

        public IDocument Create()
        {
            return new DataModelViewerViewModel(_contentManagerProvider, _eventAggregator);
        }

        public async Task New(IDocument document, string name)
        {
            var dataModelViewerViewModel = (DataModelViewerViewModel)document;
            dataModelViewerViewModel.Editor = Editors.First(x => x.FileExtensions.Contains(Path.GetExtension(name)));
            await dataModelViewerViewModel.New(name);
        }

        public async Task Open(IDocument document, string path)
        {
            var dataModelViewerViewModel = (DataModelViewerViewModel)document;
            dataModelViewerViewModel.Editor = Editors.First(x => x.FileExtensions.Contains(Path.GetExtension(path)));
            await dataModelViewerViewModel.Load(path);
        }
    }
}