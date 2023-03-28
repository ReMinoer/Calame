using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Gemini;
using Gemini.Framework.Services;
using Simulacra.IO.Watching;

namespace Calame
{
    public class CalameBootstrapper : AppBootstrapper
    {
        private readonly BitmapImage _icon;

        public CalameBootstrapper()
        {
            _icon = new BitmapImage();
            _icon.BeginInit();
            _icon.UriSource = new Uri("https://cdn3.iconfinder.com/data/icons/wpzoom-developer-icon-set/500/86-64.png");
            _icon.EndInit();
        }

        protected override void StartRuntime()
        {
            base.StartRuntime();

            var mainWindow = IoC.Get<IMainWindow>();
            mainWindow.Title = $"Calame - {CalameUtils.GetVersion() ?? "Development Build"}";
            mainWindow.Icon = null;
        }

        protected override void BindServices(CompositionBatch batch)
        {
            base.BindServices(batch);
            batch.AddExportedValue<IImportedTypeProvider>(new ImportedTypeProvider());
            batch.AddExportedValue(new PathWatcher());
        }

        protected override void PopulateAssemblySource()
        {
            string executableDirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var geminiCatalog = new DirectoryCatalog(executableDirectoryPath, "Gemini*.dll");
            AssemblySource.Instance.AddRange(
                geminiCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            var calameCatalog = new DirectoryCatalog(executableDirectoryPath, "Calame*.dll");
            AssemblySource.Instance.AddRange(
                calameCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            Window mainWindow = Application.MainWindow;
            mainWindow.Top = 0;
            mainWindow.Left = 0;
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Icon = _icon;

            OnBeforeOpeningDocuments();

            IEditorProvider[] editorProviders = null;
            foreach (string commandLineArgument in Environment.GetCommandLineArgs().Skip(1))
            {
                if (File.Exists(commandLineArgument))
                {
                    var shell = (IShell)GetInstance(typeof(IShell), null);
                    editorProviders = editorProviders ?? GetAllInstances(typeof(IEditorProvider)).Cast<IEditorProvider>().ToArray();

                    shell.OpenFileAsync(commandLineArgument, editorProviders);
                }
            }
        }

        protected virtual void OnBeforeOpeningDocuments()
        {
        }
    }
}