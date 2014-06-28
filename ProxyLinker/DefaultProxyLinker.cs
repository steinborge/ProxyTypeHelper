using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProxyHelper
{
    public class ProxyLinker
    {
        protected List<PropertyChangedEventHandler> _PropertyChangedSubscribers = new List<PropertyChangedEventHandler>();

        protected PropertyChangedEventHandler _PropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!_PropertyChangedSubscribers.Contains(value))
                {
                    _PropertyChanged += value;
                    _PropertyChangedSubscribers.Add(value);
                }
            }
            remove
            {
                _PropertyChanged -= value;
                _PropertyChangedSubscribers.Remove(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyChanged"></param>
        protected virtual void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged)
        {
            PropertyChanged += propertyChanged;
        }


        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

