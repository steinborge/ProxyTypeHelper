using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Reflection;

namespace ProxyHelper
{

    public interface ICustomObservableCollection
    {
        Type ProxyType { get; set; }
        Type ModelType { get; set; }
        object ProxyObject { get; set; }
        MethodInfo CustomCollectionChangedMethod { get; set; }
        //MethodInfo CustomCollectionChanged { get; set; }  //event NotifyCollectionChangedEventHandler 
         event NotifyCollectionChangedEventHandler CustomCollectionChanged;
    }

    public interface IIsValid
    {
        bool IsValid();
    }

    public interface ICustomObjectProxyObjects
    {
        void SetValue(Type proxyType, object value);
        Dictionary<string, object>  GetProxyObjects();
        object GetPropertyValue(string propertyName);

    }

    public interface IIsDirty
    {
        bool IsDirty();
    }

    public class LookupValue
    {
        public object Name{ get; set; }
        public object Value { get; set; }
    }

    public interface ILookupData
    {
        Dictionary<string, LookupValue> LookupValues();
    }


    class CustomObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, ICustomObservableCollection
    {
        public Type ProxyType { get; set; }
        public Type ModelType { get; set; }
        public object ProxyObject { get; set; }
        public MethodInfo  CustomCollectionChangedMethod {get;set;}
        //public event NotifyCollectionChangedEventHandler CollectionChangedEvent;
        public event NotifyCollectionChangedEventHandler CustomCollectionChanged;
    }

    

}
