using Microsoft.Web.WebView2.Wpf;
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

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        WebView2 _webView;
        webview2backhost.WebWindow mpew;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mpew = new webview2backhost.WebWindow(this);

            //mpew.WebView2Loaded += Mpew_WebView2Loaded;
            mpew.Title = "";
            mpew.Show();
 
            _webView = await mpew.WebView2ControlAsync();
            _webView.Source = new Uri("https://gdm.no/offline");
            
            this.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)); // Background="#01ffffff"
            //this.Background = new SolidColorBrush(Colors.Transparent); //use this to allow mouse actions through your window.
            Storyboard sb =(Storyboard) this.Resources["sb"];
            sb.Begin();
        }


        private void Mpew_WebView2Loaded(object sender, webview2backhost.WebView2LoadedEventArgs e)
        {
            _webView = e.WebView2Control;
            _webView.Source = new Uri("https://gdm.no/offline");
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Mouse top!");
        }
    }
}
