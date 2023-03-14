using System;
using System.Windows;

namespace Calame.AutoUpdate
{
    /// <summary>
    /// Interaction logic for WebBrowserDialog.xaml
    /// </summary>
    public partial class WebBrowserDialog : Window
    {
        public WebBrowserDialog(Uri uri)
        {
            InitializeComponent();
            WebView.Source = uri;
        }
    }
}
