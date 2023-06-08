# webview2_backgroundwindow
 Hosting WebView2 in a Window behind wpf .net48 application window to fix airspace issue.
 Solution with webview2backhost nuget-source and Sample project on github.

# webview2backhost
WebView2 with Edge is fast and glorious, but you cannot render controls over the damned thing in wpf .net48, like cefsharp. 
This hacky fix puts WebView2 in a Window behind your application, so that you can put render controls over it.  
  
My scenario was that I needed to play a stream (hls, m38u) in wpf .net48, but found issues with libs like vlc.dotnet, ffme.windows, flyleaf and xaml island hosted uwp mediaplayerelement. So I use this to play videos in video.js in an edge window sitting behind my app.

## Interacting with the webview 

The WebView2 control is exposed in the WebView2Control property. 
Use  WebView2ControlAsync method to await loading, or use the WebView2Loaded event.  

If you want to block the mouse actions pass-through set Background to almost transparent: Background="#01ffffff"  
  

If you set _your_ Window to Transparent, mouse interactions are passed through to the WebView2.<br>
I only needed to show the webview and not interact with it, so I haven't tested exstentively.  



## Usage, code sample
 
 in xaml: 
 &lt;Window [...] AllowsTransparency="True" Loaded="Window_Loaded" WindowStyle="None" Background="Transparent" &gt;


 code-behind:
  

    using Microsoft.Web.WebView2.Wpf;
(...)

        WebView2 _webView;

        webview2backhost.WebWindow mpew;
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mpew = new webview2backhost.WebWindow(this);
                //mpew.WebView2Loaded += Mpew_WebView2Loaded; //alernative to async-waiting for webview to load
            mpew.Show();
            _webView = await mpew.WebView2ControlAsync();
            _webView.Source = new Uri("https://gdm.no/offline");
                      
            this.Background = new SolidColorBrush(Colors.Transparent); //Background="Transparent", use this to allow mouse actions through your window.
            //this.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)); // Background="#01ffffff", use this to block mouse actions for the webview2
        }


        private void Mpew_WebView2Loaded(object sender, webview2backhost.WebView2LoadedEventArgs e)
        {
            _webView = e.WebView2Control;
            _webView.Source = new Uri("https://www.google.com/search?q=how+to+kill+all+humans");
        }
 

#### Credit
SetWindowPos from Utils.NativeMehods.cs from SuRGeoNix's FlyLeaf is used for convenience to set z-index of the windows. Thanks!
 *  https://github.com/SuRGeoNix/Flyleaf/blob/master/FlyleafLib/Utils/NativeMethods.cs