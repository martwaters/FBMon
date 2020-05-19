using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlitzMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Logger log = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //App.Current.NotifyIcon.
            TaskbarIcon tbIcon = ((App)Application.Current).GetIcon();
            string msg = "Message";

            FlitzBalloon balloon = new FlitzBalloon();
            balloon.BalloonText( msg);
            balloon.ShowText("The Show ...");

            //show and close after 2.5 seconds
            tbIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);

        }
    }
}
