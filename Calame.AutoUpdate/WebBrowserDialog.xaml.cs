using System;
using System.Windows;
using Microsoft.Web.WebView2.Wpf;

namespace Calame.AutoUpdate
{
    /// <summary>
    /// Interaction logic for WebBrowserDialog.xaml
    /// </summary>
    public partial class WebBrowserDialog : Window
    {
        public WebBrowserDialog(Uri uri, string userDataFolder)
        {
            InitializeComponent();

            WebView.CreationProperties = new CoreWebView2CreationProperties
            {
                UserDataFolder = userDataFolder
            };
            
            WebView.Source = uri;
        }
    }
}
