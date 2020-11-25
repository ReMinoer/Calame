using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        static public Task NewAsync(this IShell shell, IEditorProvider editorProvider, string name)
        {
            IDocument document = editorProvider.Create();

            var viewAware = (IViewAware)document;
            viewAware.ViewAttached += (sender, e) =>
            {
                var view = (FrameworkElement)e.View;
                view.Loaded += LoadedHandler;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    view.Loaded -= LoadedHandler;
                    await editorProvider.New(document, name);
                }
            };

            return shell.OpenDocumentAsync(document);
        }

        static public async Task OpenFileAsync(this IShell shell, string filePath, IEnumerable<IEditorProvider> editorProviders, string workingDirectory = null)
        {
            IEditorProvider provider = editorProviders.FirstOrDefault(p => p.Handles(filePath));
            if (provider == null)
            {
                if (!Path.HasExtension(filePath) && !File.Exists(filePath))
                {
                    string assetName = Path.GetFileName(filePath);
                    string folderPath = Path.GetDirectoryName(filePath) ?? throw new ArgumentException();

                    if (!Path.IsPathRooted(folderPath))
                        folderPath = Path.Combine(workingDirectory ?? Environment.CurrentDirectory, folderPath);

                    filePath = Directory.EnumerateFiles(folderPath, $"{assetName}.*").FirstOrDefault();
                    if (filePath == null)
                        return;
                }
                
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