using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace Calame.SceneViewer
{
    [Export(typeof(IEditorProvider))]
    public class SessionProvider : IEditorProvider
    {
        private readonly CompositionContainer _compositionContainer;
        
        public List<ISession> Sessions { get; } = new List<ISession>();
        public IEnumerable<EditorFileType> FileTypes => Sessions.Select(x => new EditorFileType(x.DisplayName, null));

        [ImportingConstructor]
        public SessionProvider(CompositionContainer compositionContainer, [ImportMany] IEnumerable<ISession> gameDataEnumerable = null)
        {
            _compositionContainer = compositionContainer;

            if (gameDataEnumerable != null)
                Sessions.AddRange(gameDataEnumerable);
        }

        public bool Handles(string path)
        {
            return false;
        }

        public IDocument Create()
        {
            return _compositionContainer.GetExportedValue<SceneViewerViewModel>();
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