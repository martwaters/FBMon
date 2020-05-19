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
            //Thread th = Thread.CurrentThread;
            //th.SetApartmentState(ApartmentState.STA);

            base.OnStartup(e);

            log = LogManager.GetLogger("Monitor");

            log.Info("########### User {0} starting {1} ###########", Environment.UserName, AppDomain.CurrentDomain.FriendlyName);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}
