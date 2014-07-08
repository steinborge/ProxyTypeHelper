using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProxyHelper
{
    public class ProxyLinker
    {
        protected PropertyChangedEventHandler _PropertyChanged;

        //TODO Add some cleanup logic to ensure all subscribers are gone- add to interface  

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                //make sure not subscribing twice
                if (_PropertyChanged == null ||
                    Array.FindIndex(_PropertyChanged.GetInvocationList(), 
                                    a => (PropertyChangedEventHandler) a == value) == -1)
                {
                    _PropertyChanged += value;
                }
            }
            remove { _PropertyChanged -= value; }
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

        protected ICommand CreateActionCommandWithArguments(Type actionMethodType, System.Type[] types, object[] param)
        {
            return actionMethodType.GetConstructor(types).Invoke(param) as ICommand;
        }

        protected ICommand CreateActionCommand(System.Action action)
        {
            return new Core.RelayCommand(action);
        }
    }
}

