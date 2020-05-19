using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FlitzMonitor
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {

        CallMonitor.Monitor FritzMon;

        public NotifyIconViewModel()
        {
            FritzMon = new CallMonitor.Monitor();
            FritzMon.Called += Fritz_Called;
            FritzMon.Connected += Fritz_Connected;
            FritzMon.Ringed += Fritz_Ringed;
            FritzMon.Disconnected += Fritz_Disconnected;

            FritzMon.Run("192.168.178.1");
        }

        private void Bubble(string what, string details)
        {
            TaskbarIcon tbIcon = ((App)Application.Current).GetIcon();

            FlitzBalloon balloon = new FlitzBalloon();
            balloon.BalloonText(what);
            balloon.ShowText(details);
            ((App)Application.Current).CallEvents.Add(what + " - " + details);

            //show and close after 2.5 seconds
            tbIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);

            //Thread th = new Thread(() =>
            //{
            //    tbIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000); 
            //});
            //th.SetApartmentState(ApartmentState.STA);
            //th.Start();
        }
        private void Fritz_Disconnected(object sender, CallMonitor.DisconnectEventArgs e)
        {
            Bubble("DISCONNECT", string.Format($"{e.TimeStamp}:{e.ConnectionId} after {e.DurationSeconds} [s]"));
        }

        private void Fritz_Ringed(object sender, CallMonitor.RingEventArgs e)
        {
            Bubble("RING", string.Format($"{e.TimeStamp}:{e.ConnectionId} - '{e.RemoteNumber}' to '{e.LocalNumber}'"));
        }

        private void Fritz_Connected(object sender, CallMonitor.ConnectEventArgs e)
        {
            Bubble("CONNECT", string.Format($"{e.TimeStamp}:{e.ConnectionId} - '{e.LocalExtension}' to '{e.RemoteNumber}'"));
        }

        private void Fritz_Called(object sender, CallMonitor.CallEventArgs e)
        {
            Bubble("CALL", string.Format($"{e.TimeStamp}:{e.ConnectionId} to '{e.LocalExtension}','{e.LocalNumber}' from '{e.RemoteNumber}'"));
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Close(),
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand {CommandAction = () => Application.Current.Shutdown()};
            }
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null  || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
