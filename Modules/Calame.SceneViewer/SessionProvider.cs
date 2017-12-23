using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.SceneViewer
{
    [Export(typeof(IEditorProvider))]
    public class SessionProvider : IEditorProvider
    {
        private readonly IShell _shell;
        private readonly ContentManagerProvider _contentManagerProvider;
        private readonly IEventAggregator _eventAggregator;
        
        public List<ISession> Sessions { get; } = new List<ISession>();
        public IEnumerable<EditorFileType> FileTypes => Sessions.Select(x => new EditorFileType(x.DisplayName, null));

        [ImportingConstructor]
        public SessionProvider(IShell shell, ContentManagerProvider contentManagerProvider, IEventAggregator eventAggregator, [ImportMany] IEnumerable<ISession> gameDataEnumerable = null)
        {
            _shell = shell;
            _contentManagerProvider = contentManagerProvider;
            _eventAggregator = eventAggregator;

            if (gameDataEnumerable != null)
                Sessions.AddRange(gameDataEnumerable);
        }

        public bool Handles(string path)
        {
            return false;
        }

        public IDocument Create()
        {
            return new SceneViewerViewModel(_shell, _contentManagerProvider, _eventAggregator);
        }

        public async Task New(IDocument document, string name)
        {
            var sceneViewerViewModel = (SceneViewerViewModel)document;
            sceneViewerViewModel.Session = Sessions.First();
            sceneViewerViewModel.InitializeSession();
        }

        public async Task Open(IDocument document, string path)
        {
        }
    }
}