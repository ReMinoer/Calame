using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using Calame.Icons;
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
        private readonly IIconProvider _iconProvider;
        private readonly IIconDescriptor<ISession> _iconDescriptor;

        public ISession[] Sessions { get; }
        public IDataSession[] DataSessions { get; }
        public IEnumerable<EditorFileType> FileTypes => Sessions.Select(x => new EditorFileType(x.DisplayName, null, _iconProvider.GetUri(_iconDescriptor.GetIcon(x), 16)));

        bool IEditorProvider.CanCreateNew => true;

        [ImportingConstructor]
        public SessionProvider(CompositionContainer compositionContainer, IIconProvider iconProvider, IIconDescriptorManager iconDescriptorManager,
            [ImportMany] ISession[] sessions = null, [ImportMany] IDataSession[] dataSession = null)
        {
            _compositionContainer = compositionContainer;
            _iconProvider = iconProvider;
            _iconDescriptor = iconDescriptorManager.GetDescriptor<ISession>();

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
            return New(document, Sessions.First());
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