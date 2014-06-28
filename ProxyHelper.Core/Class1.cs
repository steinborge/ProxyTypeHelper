using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProxyHelper.Core
{
    public class ProxyLinker
    {
        //*public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged = delegate { };
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

