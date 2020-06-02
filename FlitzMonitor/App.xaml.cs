using Hardcodet.Wpf.TaskbarNotification;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace FlitzMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public TaskbarIcon GetIcon()
        { 
            return notifyIcon;
        }
        private TaskbarIcon notifyIcon;

        public List<string> CallEvents = new List<string>();

        Logger log;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += DispatchUnhandledException;
            log = LogManager.GetLogger("Monitor");

            log.Info("########### User {0} starting {1} ###########", Environment.UserName, AppDomain.CurrentDomain.FriendlyName);

            try
            {
                //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
                notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            }
            catch(Exception ex)
            {
                log.Error($"Shutdown with issue: {Tools.GetInnerMostMessage(ex)}");
                this.Shutdown();
            }
        }

        private void DispatchUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            log.Error($"Exit with unhandled issue: {e.Exception.Message}");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log.Info($"Closing {AppDomain.CurrentDomain.FriendlyName}");
            if(notifyIcon != null)
                notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
