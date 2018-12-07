using System.Windows.Input;
using Calame.DataModelViewer.ViewModels;
using Calame.Viewer;
using Glyph.WpfInterop;

namespace Calame.DataModelViewer.Views
{
    public partial class DataModelViewerView : IViewerView
    {
        public IWpfGlyphClient Client => GlyphWpfViewer;

        public DataModelViewerView()
        {
            InitializeComponent();
        }

        private void Viewer_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Tab)
                e.Handled = true;
        }

        private void Viewer_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Tab)
                e.Handled = true;
        }
    }
}
