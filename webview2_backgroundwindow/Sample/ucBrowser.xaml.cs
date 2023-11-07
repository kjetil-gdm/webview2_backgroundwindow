using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using mediaplayerelementhost;
using Microsoft.Web.WebView2.Wpf;

namespace mediaplayerelementhost
{
    /// <summary>
    /// Interaction logic for ucBrowser.xaml
    /// </summary>
    public partial class ucBrowser : UserControl
    {
        public ucBrowser()
        {
            InitializeComponent();
        }
 
        WebView2 _webView;
        webview2backhost.WebWindow mpew;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mpew = new webview2backhost.WebWindow(Application.Current.MainWindow);

            //mpew.WebView2Loaded += Mpew_WebView2Loaded;
            mpew.Title = "";
            mpew.Show();

            _webView = await mpew.WebView2ControlAsync();
            _webView.Source = new Uri("https://gdm.no/offline");
 
        }


        private void Mpew_WebView2Loaded(object sender, webview2backhost.WebView2LoadedEventArgs e)
        {
            _webView = e.WebView2Control;
            _webView.Source = new Uri("https://gdm.no/offline");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            mpew.Close();

        }
    }
}
