using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Gemini;
using MonoGame.Framework.WpfInterop;

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
        
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            Window mainWindow = Application.MainWindow;
            if (mainWindow == null)
                return;
            
            mainWindow.Title = "Calame";
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Icon = _icon;
        }

        protected override void BindServices(CompositionBatch batch)
        {
            base.BindServices(batch);
            batch.AddExportedValue(new ContentManagerProvider(D3D11Client.GraphicsDevice));
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
                throw;
            }
        }
#endif
    }
}