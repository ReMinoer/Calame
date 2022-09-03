using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
            mainWindow.Title = "Calame";
            mainWindow.Icon = null;
        }

        protected override void BindServices(CompositionBatch batch)
        {
            base.BindServices(batch);
            batch.AddExportedValue<IImportedTypeProvider>(new ImportedTypeProvider());
            batch.AddExportedValue(new PathWatcher());
        }

#if DEBUG
        protected override void Configure()
        {
            try
            {
                base.Configure();
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
#endif

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            Window mainWindow = Application.MainWindow;
            mainWindow.Top = 0;
            mainWindow.Left = 0;
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Icon = _icon;

            foreach (string commandLineArgument in Environment.GetCommandLineArgs().Skip(1))
            {
                if (File.Exists(commandLineArgument))
                {
                    var shell = (IShell)GetInstance(typeof(IShell), null);
                    IEnumerable<IEditorProvider> editorProviders = GetAllInstances(typeof(IEditorProvider)).Cast<IEditorProvider>();
                    shell.OpenFileAsync(commandLineArgument, editorProviders);
                }
            }
        }
    }
}