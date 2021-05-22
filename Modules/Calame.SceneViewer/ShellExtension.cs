using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Glyph.Composition.Modelization;

namespace Calame.SceneViewer
{
    static public class ShellExtension
    {
        static public Task NewSessionAsync<TSession>(this IShell shell)
            where TSession : ISession
        {
            var sessionProvider = IoC.Get<SessionProvider>();
            return shell.ShowDocumentAsync(sessionProvider, document => sessionProvider.New<TSession>(document));
        }

        static public Task NewDataSessionAsync<TDataSession, TData>(this IShell shell, TData data)
            where TDataSession : IDataSession<TData>
            where TData : IGlyphData
        {
            var sessionProvider = IoC.Get<SessionProvider>();
            return shell.ShowDocumentAsync(sessionProvider, document => sessionProvider.New<TDataSession, TData>(document, data));
        }
    }
}