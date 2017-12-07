using System.Windows.Input;

namespace Calame.DataModelViewer.Views
{
    public partial class DataModelViewerView
    {
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
