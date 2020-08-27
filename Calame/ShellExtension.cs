using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Simulacra.IO.Utils;

namespace Calame
{
    static public class ShellExtension
    {
        static public async Task OpenFileAsync(this IShell shell, string filePath, IEnumerable<IEditorProvider> editorProviders)
        {
            IEditorProvider provider = editorProviders.FirstOrDefault(p => p.Handles(filePath));
            if (provider == null)
            {
                Process.Start(filePath);
                return;
            }

            IDocument alreadyOpenedDocument = shell.Documents.OfType<IPersistedDocument>().FirstOrDefault(x => new PathComparer().Equals(x.FilePath, filePath));
            if (alreadyOpenedDocument != null)
            {
                await shell.OpenDocumentAsync(alreadyOpenedDocument);
                return;
            }

            IDocument editor = provider.Create();

            var viewAware = (IViewAware)editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var view = (FrameworkElement)e.View;
                view.Loaded += LoadedHandler;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    view.Loaded -= LoadedHandler;
                    await provider.Open(editor, filePath);
                }
            };

            await shell.OpenDocumentAsync(editor);
        }
    }
}