

using mediaplayerelementhost;
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

       
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Mouse top!");
        }

        ucBrowser _ucBrowser;
        private void Button_Click(object sender, RoutedEventArgs e)
        {

              _ucBrowser = new ucBrowser();

            this.grid.Children.Add(_ucBrowser);
            this.btnKill.Visibility = Visibility.Visible;
            //this.Background = new SolidColorBrush(Colors.Transparent); //use this to allow mouse actions through your window.
            Storyboard sb = (Storyboard)this.Resources["sb"];
            sb.Begin();

            this.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255)); // Background="#01ffffff"

        }

        private void btnKill_Click(object sender, RoutedEventArgs e)
        {
            
            this.grid.Children.Clear();
            this.btnKill.Visibility =Visibility.Collapsed;
            //this.Background = Brushes.HotPink;
            this.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
        }
    }
}
