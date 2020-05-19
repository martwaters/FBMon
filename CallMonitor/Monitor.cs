using FritzBoxCallMonitor;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CallMonitor
{
    /// <summary>
    /// class monitors fritzbox phone events
    /// </summary>
    public class Monitor
    {
        static Logger Log = LogManager.GetCurrentClassLogger();

        public event EventHandler<ConnectEventArgs> Connected;
        public event EventHandler<CallEventArgs> Called;
        public event EventHandler<RingEventArgs> Ringed;
        public event EventHandler<DisconnectEventArgs> Disconnected;

        /// <summary>
        /// Runs the specified fritz box detection loop.
        /// </summary>
        /// <param name="fritzBox">The fritz box.</param>
        public void Run(string fritzBox)
        {
            Log.Debug("Run FritzBoxCallMonitor on {0}", fritzBox);

            IPAddress fritzBoxIp = null;
            IPAddress.TryParse(fritzBox, out fritzBoxIp);
            if (fritzBoxIp == null)
                fritzBoxIp = Dns.GetHostEntry(fritzBox).AddressList.First();

            EventDrivenTcpClient tcpClient = new EventDrivenTcpClient(fritzBoxIp, 1012, true);
            tcpClient.DataReceived += tcpClient_DataReceived;
            tcpClient.ConnectionStatusChanged += tcpClient_ConnectionStatusChanged;
            tcpClient.ReconnectInterval = 30000;
            tcpClient.Connect();
        }
        private void tcpClient_ConnectionStatusChanged(EventDrivenTcpClient sender, EventDrivenTcpClient.ConnectionStatus status)
        {
            Log.Debug("Connection status changed: {0}", status);
        }

        private void tcpClient_DataReceived(EventDrivenTcpClient sender, object data)
        {
            Log.Debug("Message received: {0}", data);

            //Outgoing call:             Date;CALL;ConnectionID;LocalExtension;LocalNumber;RemoteNumber;
            //Incoming call:             Date;RING;ConnectionID;RemoteNumber;LocalNumber;
            //On connection established: Date;CONNECT;ConnectionID;LocalExtension;RemoteNumber;
            //On connection end:         Date;DISCONNECT;ConnectionID;DurtionInSeconds;

            string[] messageParts = data.ToString().Split(';');
            DateTime timestamp = DateTime.Parse(messageParts[0]);
            string eventType = messageParts[1];
            string connectionId = messageParts[2];

            switch (eventType)
            {
                case "CALL":
                    Called?.Invoke(sender, new CallEventArgs()
                    {
                        TimeStamp = timestamp,
                        ConnectionId = connectionId,
                        LocalExtension = messageParts[3],
                        LocalNumber = messageParts[4],
                        RemoteNumber = messageParts[5],
                    });
                    break;
                case "RING":
                    Ringed?.Invoke(sender, new RingEventArgs()
                    {
                        TimeStamp = timestamp,
                        ConnectionId = connectionId,
                        RemoteNumber = messageParts[3],
                        LocalNumber = messageParts[4],
                    });
                    break;
                case "CONNECT":
                    Connected?.Invoke(sender, new ConnectEventArgs()
                    {
                        TimeStamp = timestamp,
                        ConnectionId = connectionId,
                        LocalExtension = messageParts[3],
                        RemoteNumber = messageParts[4],
                    });
                    break;
                case "DISCONNECT":
                    Disconnected?.Invoke(sender, new DisconnectEventArgs()
                    {
                        TimeStamp = timestamp,
                        ConnectionId = connectionId,
                        DurationSeconds = int.Parse(messageParts[3]),
                    });
                    break;
            }
        }

    }
}
