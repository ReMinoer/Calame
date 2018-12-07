using System.Windows.Input;
using Calame.Viewer;
using Glyph.WpfInterop;

namespace Calame.SceneViewer.Views
{
    public partial class SceneViewerView : IViewerView
    {
        public IWpfGlyphClient Client => GlyphWpfViewer;

        public SceneViewerView()
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
