using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallMonitor
{
    public class MonitorEventArgs : EventArgs
    {
        public DateTime TimeStamp { get; set; }
        public string ConnectionId { get; set; }
    }

    public class ConnectEventArgs : MonitorEventArgs
    {
        public string LocalExtension { get; set; }
        public string RemoteNumber { get; set; }
    }

    public class CallEventArgs : MonitorEventArgs
    {
        public string LocalExtension { get; set; }
        public string LocalNumber { get; set; }
        public string RemoteNumber { get; set; }
    }

    public class RingEventArgs : MonitorEventArgs
    {
        public string LocalNumber { get; set; }
        public string RemoteNumber { get; set; }
    }

    public class DisconnectEventArgs : MonitorEventArgs
    {
        public int DurationSeconds { get; set; }
    }
}
