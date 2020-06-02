using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using Number2Name;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FlitzMonitor
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        #region Property Changed

        /// <summary>
        /// Fired when a project property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Notify that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        /// <summary>
        /// Set field value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field</param>
        /// <param name="value">The value</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>If set was successful</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            //if (!IsDirty)
            //{
            //    if (!propertyName.Equals(nameof(IsDirty))) IsDirty = true;
            //}
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion // Property Changed

        static Logger Log = LogManager.GetCurrentClassLogger();
        CallMonitor.Monitor FritzMon;
        CallerNames Names;

        public NotifyIconViewModel()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 3)
            {
                throw new Exception("Flitz Id args required!");
            }
            Names = new CallerNames(args[1], args[2]);
            FritzMon = new CallMonitor.Monitor();
            FritzMon.Called += Fritz_Called;
            FritzMon.Connected += Fritz_Connected;
            FritzMon.Ringed += Fritz_Ringed;
            FritzMon.Disconnected += Fritz_Disconnected;

            FritzMon.Run(Properties.Settings.Default.FritzAddress);
        }

        public ObservableCollection<FBEventItem> BoxEvents { get => boxEvents; set => SetField(ref boxEvents, value); }
        private ObservableCollection<FBEventItem> boxEvents = new ObservableCollection<FBEventItem>();

        private void Bubble(string id, DateTime timeStamp, string what, string details)
        {
            object fest = new object();
            lock (fest)
            {
                Log.Info($"({id}) {what}: {details}");

                // store the event for the monitor list
                FBEventItem fbEvent = new FBEventItem()
                {
                    TimeStamp = timeStamp,
                    Category = what,
                    Text = details,
                    Id = id
                };

                // prevent threading crash ...
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    Update(fbEvent);
                });
            }
        }
        private void Update(FBEventItem fbEvent)
        {
            BoxEvents.Add(fbEvent);
            FlitzBalloon balloon = new FlitzBalloon();
            balloon.BalloonText(fbEvent.Category);
            balloon.ShowText(fbEvent.Text);

            //show and close after 2.5 seconds
            TaskbarIcon tbIcon = ((App)Application.Current).GetIcon();
            tbIcon.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
        }

        private void Fritz_Disconnected(object sender, CallMonitor.DisconnectEventArgs e)
        {
            Bubble( e.ConnectionId, e.TimeStamp, "Ende", string.Format($"nach {e.DurationSeconds} [s]"));
        }

        private void Fritz_Ringed(object sender, CallMonitor.RingEventArgs e)
        {
            Bubble(e.ConnectionId, e.TimeStamp, "->Eingehend", string.Format($"von '{Names.Resolve(e.RemoteNumber)}' an '{e.LocalNumber}'"));
        }

        private void Fritz_Connected(object sender, CallMonitor.ConnectEventArgs e)
        {
            Bubble(e.ConnectionId, e.TimeStamp, "Verbindung", string.Format($"'{Names.GetInternal(e.LocalExtension)}' mit '{Names.Resolve(e.RemoteNumber)}'"));
        }

        private void Fritz_Called(object sender, CallMonitor.CallEventArgs e)
        {
            Bubble(e.ConnectionId, e.TimeStamp, "Ausgehend->", string.Format($"von '{Names.GetInternal(e.LocalExtension)}','{e.LocalNumber}' an '{Names.Resolve(e.RemoteNumber)}'"));
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
