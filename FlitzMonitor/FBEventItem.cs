using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FlitzMonitor
{
    public class FBEventItem : INotifyPropertyChanged
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

        //internal void PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    if (sender is PasswordBox pbx)
        //        Password = pbx.Password;
        //}

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

        public DateTime TimeStamp { get => timeStamp; set => SetField(ref timeStamp, value); }
        private DateTime timeStamp;

        public string Category { get => category; set => SetField(ref category, value); }
        private string category;

        public string Id { get => id; set => SetField(ref id, value); }
        private string id;

        public string Text { get => text; set => SetField(ref text, value); }
        private string text;
    }
}
