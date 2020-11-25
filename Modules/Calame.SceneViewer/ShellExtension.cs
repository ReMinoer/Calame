using System;
using System.Threading.Tasks;
using System.Windows;
using Calame.SceneViewer.ViewModels;
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
            IDocument document = sessionProvider.Create();

            var viewAware = (IViewAware)document;
            viewAware.ViewAttached += (sender, e) =>
            {
                var view = (FrameworkElement)e.View;
                view.Loaded += LoadedHandler;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    view.Loaded -= LoadedHandler;
                    await sessionProvider.New<TSession>(document);
                }
            };

            return shell.OpenDocumentAsync(document);
        }

        static public Task NewDataSessionAsync<TDataSession, TData>(this IShell shell, TData data)
            where TDataSession : IDataSession<TData>
            where TData : IGlyphData
        {
            var sessionProvider = IoC.Get<SessionProvider>();
            IDocument document = sessionProvider.Create();

            var viewAware = (IViewAware)document;
            viewAware.ViewAttached += (sender, e) =>
            {
                var view = (FrameworkElement)e.View;
                view.Loaded += LoadedHandler;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    view.Loaded -= LoadedHandler;
                    await sessionProvider.New<TDataSession, TData>(document, data);
                }
            };

            return shell.OpenDocumentAsync(document);
        }
    }
}