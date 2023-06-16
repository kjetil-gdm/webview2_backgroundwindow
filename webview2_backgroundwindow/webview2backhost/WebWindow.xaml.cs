
using Microsoft.Web.WebView2.Core;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace webview2backhost
{
    public class WebView2LoadedEventArgs
    {
        public WebView2LoadedEventArgs(WebView2 _control) { WebView2Control = _control; }
        public WebView2 WebView2Control  { get; } // readonly
    }

    /// <summary>
    ///  Window for WebView2
    /// </summary>
    public partial class WebWindow : Window
    {
        public delegate void WebView2EventHandler(object sender, WebView2LoadedEventArgs e);
        public event WebView2EventHandler WebView2Loaded;


        Window _Parent = null;
        private IntPtr _ParentHandle = IntPtr.Zero;

        private IntPtr _MyHandle = IntPtr.Zero;

        public int ResizingSide { get; private set; }
 
        public WebWindow()
        {

            InitializeComponent();

        }
         
        /// <summary>
        /// Dsposes the WebView2 to prevent it from hanging around in the background and closes the hosting Window
        /// </summary>
        new public void Close()
        {
            try { 
            _webView?.Dispose();
            _webView = null;
            _Parent.SizeChanged -= _Parent_SizeChanged;
            _Parent.LocationChanged -= _Parent_LocationChanged;
            _Parent.StateChanged -= _Parent_StateChanged;
            _Parent.Closed -= _Parent_Closed;
            }catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("#Exception in WebWindow.Close() "+ ex.ToString() +Environment.NewLine+ex.StackTrace);
            }
            finally
            {
                base.Close();
            }

        }
        
        public async Task<WebView2> WebView2ControlAsync()
        {
            int timeout = 0;
            while(_loaded == false)
            {
                if(timeout > 500) { throw new Exception(">10 seconds to load webview, something must be wrong."); }
                //Console.WriteLine("Waiting for webview to load");
                await Task.Delay(20);
                timeout++;
            }
            return this._webView;
        }

        public WebView2 WebView2Control
        {
            get
            {
                return this._webView;
            }
        }

        /// <summary>
        /// Create a window that should sit behind parent at all times. Remember to set Parent Window to WindowStyle="None" and AllowsTransparency="True". Background will be set to (almost) Transparent by WebWindow.
        /// </summary>
        /// <param name="parent">Window object (the one to sit in front of the WebView2 window)</param>
        public WebWindow(Window parent)
        {
            _Parent = parent;
           
            InitializeComponent();

            _Parent.SizeChanged += _Parent_SizeChanged;
            _Parent.LocationChanged += _Parent_LocationChanged;

            _Parent.StateChanged += _Parent_StateChanged;
            _Parent.Closed += _Parent_Closed;
            //_Parent.Activated += _Parent_Activated;
            Window_Align();
        
        }

 
        private void _Parent_Closed(object sender, EventArgs e)
        {
            //Console.WriteLine("_Parent_Closed");
            this.Close();
        }

        private void _Parent_StateChanged(object sender, EventArgs e)
        {
          //  Console.WriteLine("_Parent_StateChanged : " + _Parent.WindowState.ToString() );
            Window_Align();
        }

        private void _Parent_LocationChanged(object sender, EventArgs e)
        {
         //   Console.WriteLine("_Parent_LocationChanged");
            Window_Align();
        }

        private void _Parent_Activated(object sender, EventArgs e)
        {
           // Console.WriteLine("_Parent_Activated");
            Window_Align();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Mouse down on webview window!");
        }

        private void _Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // Console.WriteLine("_Parent_SizeChanged");
            Window_Align();
        }
 
        private void window_ContentRendered(object sender, EventArgs e)
        {
            addWebView();

        }

        private bool _loaded = false; 
        protected virtual void RaiseLoadedEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            WebView2Loaded?.Invoke(this, new WebView2LoadedEventArgs(this._webView));
        }

        private async void addWebView()
        {
            var options = new CoreWebView2EnvironmentOptions("--autoplay-policy=no-user-gesture-required");
            //var path = @"C:\Program Files (x86)\Microsoft\EdgeWebView\Application\113.0.1774.50";
            //path is first param in CreateAsync, but with 'null' the path is 
            var environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            //path,null,options
  
            //this must be done before .Source is set first thime on the webview2: 
            await this._webView.EnsureCoreWebView2Async(environment);
            _loaded = true;
            RaiseLoadedEvent();

            Window_Align();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("WebWindow_Loaded");

            _ParentHandle = new WindowInteropHelper(_Parent).EnsureHandle();
            _MyHandle = new WindowInteropHelper(this).EnsureHandle();

            //_Parent.Owner = this;
             int exStyle = (int)Utils.NativeMethods.GetWindowLong(_MyHandle, (int)Utils.NativeMethods.WindowLongFlags.GWL_EXSTYLE);
              exStyle |= (int)Utils.NativeMethods.WindowStylesEx.WS_EX_TOOLWINDOW;

              Utils.NativeMethods.SetWindowLong(_MyHandle, (int)Utils.NativeMethods.WindowLongFlags.GWL_EXSTYLE, (IntPtr)exStyle);
 
            Window_Align();
        }
 
       //int align_count = 0;
        private void Window_Align()
        {

 

           //align_count++;
           //Console.WriteLine("Window Align "+ align_count);

            Application.Current.Dispatcher.Invoke(new Action(() => {

                /* DPI scaling stuff, not needed currently: 
                 * PresentationSource source = PresentationSource.FromVisual(this);
                double dpiX, dpiY, dpiScaleX, dpiScaleY;
                if (source != null)
                {
                   dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                    dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
                  */
                //}


                if(_Parent.WindowState == WindowState.Minimized)
                {
                    this.Hide();
                }
                else
                {
                    this.WindowState = _Parent.WindowState;
                    this.Show();  
                }


                    this.Left = _Parent.Left;
                    this.Top = _Parent.Top;
                    this.Width = _Parent.ActualWidth; 
                    this.Height = _Parent.ActualHeight;
                int exStyle = (int)Utils.NativeMethods.GetWindowLong(_MyHandle, (int)Utils.NativeMethods.WindowLongFlags.GWL_EXSTYLE);
                exStyle |= (int)Utils.NativeMethods.WindowStylesEx.WS_EX_TOOLWINDOW;

                Utils.NativeMethods.SetWindowLong(_MyHandle, (int)Utils.NativeMethods.WindowLongFlags.GWL_EXSTYLE, (IntPtr)exStyle);

                /*Console.WriteLine("this.Width: " + this.Width + ", Parent: " + _Parent.Width + ", Actual: " + _Parent.ActualWidth);
                Console.WriteLine("this.Height: " + this.Height + ", Parent: " + _Parent.Height + ", Actual: " + _Parent.ActualHeight);
                Console.WriteLine("this.Left: " + this.Left + ", Parent: " + _Parent.Left);
                Console.WriteLine("this.Top: " + this.Top + ", Parent: " + _Parent.Top); */

            }));
            //Console.WriteLine("SetWindowPos");
            //Set this Window to be on top in z-order, then set the Parent window to be top in z-order.
            Utils.NativeMethods.SetWindowPos(_ParentHandle, (IntPtr)Utils.NativeMethods.SetWindowPosFlags.SWP_TOP, 0, 0, 0, 0, (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOMOVE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOSIZE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);

            //Utils.NativeMethods.SetWindowPos(_MyHandle, (IntPtr)Utils.NativeMethods.SetWindowPosFlags.SWP_TOP, 0, 0, 0, 0, (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOMOVE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOSIZE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);

            Utils.NativeMethods.SetWindowPos(_MyHandle, _ParentHandle, 0, 0, 0, 0, (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOMOVE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOSIZE | (uint)Utils.NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);

            // Console.WriteLine("Align, new state: : " + this.WindowState.ToString() +", top: "+ this.Top.ToString());
            //Console.WriteLine("Fin.");
        }

        private void window_StateChanged(object sender, EventArgs e)
        {
            if(_Parent.WindowState != this.WindowState)
            {
                //Console.WriteLine("State mismatch");
              
                Window_Align();
            }
        }
    }
}
