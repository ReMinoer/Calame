using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Calame.SceneViewer.ViewModels;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph.Composition.Modelization;

namespace Calame.SceneViewer
{
    [Export(typeof(IEditorProvider))]
    [Export(typeof(SessionProvider))]
    public class SessionProvider : IEditorProvider
    {
        private readonly CompositionContainer _compositionContainer;
        
        public ISession[] Sessions { get; }
        public IDataSession[] DataSessions { get; }
        public IEnumerable<EditorFileType> FileTypes => Sessions.Select(x => new EditorFileType(x.DisplayName, null));

        [ImportingConstructor]
        public SessionProvider(CompositionContainer compositionContainer, [ImportMany] ISession[] sessions = null, [ImportMany] IDataSession[] dataSession = null)
        {
            _compositionContainer = compositionContainer;

            Sessions = sessions;
            DataSessions = dataSession;
        }

        public bool Handles(string path)
        {
            return false;
        }

        public IDocument Create()
        {
            return _compositionContainer.GetExportedValue<SceneViewerViewModel>();
        }

        public Task New(IDocument document, string name)
        {
            return New(document, Sessions.First(x => x.GetType().Name == name));
        }

        public Task New<TSession>(IDocument document)
            where TSession : ISession
        {
            return New(document, Sessions.OfType<TSession>().First());
        }

        public Task New<TDataSession, TData>(IDocument document, TData data)
            where TDataSession : IDataSession<TData>
            where TData : IGlyphData
        {
            TDataSession dataSession = DataSessions.OfType<TDataSession>().First();
            dataSession.Data = data;
            return New(document, dataSession);
        }

        private async Task New(IDocument document, ISession session)
        {
            var sceneViewerViewModel = (SceneViewerViewModel)document;
            sceneViewerViewModel.Session = session;
            await sceneViewerViewModel.InitializeSession();
        }

        public Task Open(IDocument document, string path)
        {
            return Task.CompletedTask;
        }
    }
}