using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlitzMonitor
{
    public class FlitzBalloonViewModel  : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string ShowText 
        {
            get { return showText; }
            set
            {
                if (showText != value)
                {
                    showText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string showText { get; set; }
        public string BalloonText
        {
            get { return balloonText; }
            set
            {
                if (balloonText != value)
                {
                    balloonText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string balloonText { get; set; }
    }
}
