using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.SceneViewer
{
    [Export(typeof(IEditorProvider))]
    public class SessionProvider : IEditorProvider
    {
        private readonly IShell _shell;
        private readonly ContentManagerProvider _contentManagerProvider;
        public List<ISession> Sessions { get; } = new List<ISession>();
        public IEnumerable<EditorFileType> FileTypes => Sessions.Select(x => new EditorFileType(x.DisplayName, null));

        [ImportingConstructor]
        public SessionProvider(IShell shell, ContentManagerProvider contentManagerProvider, [ImportMany] IEnumerable<ISession> gameDataEnumerable = null)
        {
            _shell = shell;
            _contentManagerProvider = contentManagerProvider;

            if (gameDataEnumerable != null)
                Sessions.AddRange(gameDataEnumerable);
        }

        public bool Handles(string path)
        {
            return false;
        }

        public IDocument Create()
        {
            return new SceneViewerViewModel(_shell, _contentManagerProvider);
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