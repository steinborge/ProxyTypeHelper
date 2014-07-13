﻿// based on http://blogs.msdn.com/b/silverlight_sdk/archive/2011/04/25/binding-to-dynamic-properties-with-icustomtypeprovider-silverlight-5-beta.aspxhttp://blogs.msdn.com/b/silverlight_sdk/archive/2011/04/25/binding-to-dynamic-properties-with-icustomtypeprovider-silverlight-5-beta.aspx
using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Input;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ProxyHelper
{
    /// <summary>
    /// Custom type helper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class ProxyTypeHelper<T> : ProxyLinker, ICustomTypeProvider, INotifyPropertyChanged, ICustomObjectProxyObjects, IIsDirty
    {
        static Dictionary <string,List<string>> linkedProperties = new Dictionary<string,List<string>> ();
        public static Dictionary<string, MethodInfo> linkedOC = new Dictionary<string, MethodInfo>();

  
        /// <summary>
        /// Static constructor. Maps all existing class properties to custom properties
        /// </summary>
         static ProxyTypeHelper()
        {
            Type ownerType= typeof(T);

            //enumerate all properties of type T and create proxies to intercept setters and getters..
            foreach (var property in ownerType.GetProperties())
            {
                //determine if not mapped..
                bool notMapped = property.IsDefined(typeof(NotMapped),false);




                bool linkedProperty= property.IsDefined(typeof(LinkToProperty), false);

                //if property is linked to other 
                if (linkedProperty)
                {
                    
                    object[] customAttributes = property.GetCustomAttributes(typeof(LinkToProperty), true);
                    List<string> propertyList = null;

                    foreach(LinkToProperty propertyVal in customAttributes )
                    {
                        //
                        if (!linkedProperties.TryGetValue(propertyVal.Name, out propertyList))
                        {
                            propertyList = new List<string> ();
                            linkedProperties.Add(propertyVal.Name, propertyList);
                        }

                        propertyList.Add(property.Name);

                    }
                }


                CustomPropertyInfoHelper cpi = new CustomPropertyInfoHelper(ownerType, property,notMapped);
                Type propertyType = property.PropertyType;//  property.GetType();
                cpi.IsPrimitive =   (propertyType.IsPrimitive || propertyType == typeof(Decimal)
                       || propertyType == typeof(DateTime) || propertyType == typeof(String)
                       || propertyType == typeof(DateTimeOffset) || propertyType == typeof(TimeSpan)
                       || propertyType == typeof(Guid));

                cpi.RaisePropertyChangedEvent = !property.IsDefined(typeof(DoNotRaiseProperyChangedEvents), false);
                cpi.IsProxy = !notMapped;
                cpi.ProxyObjectTypeFullName = ownerType.FullName;
                cpi.IsLocalProxy = !notMapped;  

                //TODO null check these - methods may not exist
                cpi.ProxyGetMethod = ownerType.GetMethods().Single(m => m.Name == "get_" + property.Name && !m.IsGenericMethod);
                cpi.ProxySetMethod = ownerType.GetMethod("set_" + property.Name);
                cpi.ProxyPropertyInfo = property;


                //if property has default value set via attribute
                if (property.IsDefined(typeof(DefaultValue), false))
                {


                    object defaultValue = ((DefaultValue)(property.GetCustomAttributes(typeof(DefaultValue), true)[0])).Value;
                    cpi.DefaultValue = defaultValue;
                }


                CustomProperties.Add(property.Name, cpi);
             }

             //loop through all methods and check for event wire up methods.. 
            foreach (MethodInfo method in ownerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
             {

                     //check if collection changed event is linked to a specific collection
                     if (method.IsDefined(typeof(LinkToCollection), false))
                     {
                         string collectionName = ((LinkToCollection)(method.GetCustomAttributes(typeof(LinkToCollection), true)[0])).Name;

                         linkedOC.Add(collectionName, method);
                     }

                    //check if method is property changed event..
                     if (method.IsDefined(typeof(PropertyChangedEvent), false))
                         PropertyChangedMethod = method;

                     //get all link to action attributes
                     bool isEvent = false;
                    object[] customAttributes = method.GetCustomAttributes(typeof(LinkToCommand), true);

                    //check if link to event attribute 
                    if (customAttributes == null || customAttributes.Length == 0)
                    {
                        customAttributes = method.GetCustomAttributes(typeof(LinkToEvent), true);
                        if (customAttributes.Length > 0)
                            isEvent = true; 
                    }
                    
                    //
                     if (customAttributes != null && customAttributes.Length > 0)
                     {
                         INamedAttribute linkedMethod = customAttributes[0] as  INamedAttribute;

                         CustomPropertyInfoHelper cpi = new CustomPropertyInfoHelper(linkedMethod.Name,typeof(string),typeof(T));
                         cpi.IsAction = true;
                         cpi.ActionHasArgument = isEvent;

                         //extract any arguments..
                         if (cpi.ActionHasArgument)
                         {
                             cpi.ActionArgumentType=method.GetParameters()[0].ParameterType;

                             Type genericAction = typeof(Core.WPF.Infrastructure.ActionCommand<>); 
                             cpi.ActionMethodType= genericAction.MakeGenericType(  method.GetParameters()[0].ParameterType);

                             genericAction = typeof(Action<>);
                             cpi.ActionArgumentType= genericAction.MakeGenericType(method.GetParameters()[0].ParameterType);
                         }

                         cpi.ActionMethod = method;
                         CustomProperties.Add(linkedMethod.Name, cpi);
                     }
             }

             //set linked properties
            foreach (KeyValuePair<string, List<string>> linkedProperty in linkedProperties)
            {
                CustomPropertyInfoHelper cpi = null;

                if (CustomProperties.TryGetValue(linkedProperty.Key, out cpi))
                {
                    cpi.LinkedProperties = linkedProperty.Value;
                }
            }
         }

         static MethodInfo PropertyChangedMethod { get; set; }
         
         public delegate void Propchanged(object sender, System.ComponentModel.PropertyChangedEventArgs e);

        /// <summary>
        ///  Constructor. Initialize property dictionary 
        /// </summary>
        public ProxyTypeHelper()
        {
            Type parentType = typeof(T);
            ProxyObjects = new Dictionary<string, object>();

            //if propery change event method flagged via attribute assign the method to event
            if (PropertyChangedMethod != null)
                SetPropertyChangedEventHandler((PropertyChangedEventHandler)Delegate.CreateDelegate(typeof(PropertyChangedEventHandler), this, PropertyChangedMethod, true));


            //loop through each class property and generate an instance
            foreach (var property in GetCustomType().GetProperties())
            {
                CustomPropertyInfoHelper cpi = null; 

                if (property is CustomPropertyInfoHelper)
                    cpi = property as CustomPropertyInfoHelper;
                else //otherwise class property
                    cpi = new CustomPropertyInfoHelper(property.Name, property.GetType(), parentType);


                PropertyValue propVal = new PropertyValue(null, cpi);

                if (cpi.DefaultValue != null)
                    propVal.Value = cpi.DefaultValue;


                //if the proxy object parent name is same as current object then set proxy object to value of current object
                if (cpi.IsProxy && cpi.ProxyObjectTypeFullName == parentType.FullName)
                {
                    propVal.ProxyObject = this;
                }
                else
                    if (cpi.IsProxy)//check if proxy object and if it's been stored 
                    {
                        if (!ProxyObjects.ContainsKey(cpi.ProxyObjectTypeFullName)) 
                            ProxyObjects.Add(cpi.ProxyObjectTypeFullName, null); 
                    }
                    else
                        //if it's a command or event then create delegate and appropriate object..
                        if (cpi.IsAction)
                        {
                            Type actionType = typeof(Action);
                            if (cpi.ActionHasArgument)
                            {
                                actionType = cpi.ActionArgumentType;
                            }

                            Delegate openDelegate = Delegate.CreateDelegate(actionType, this, cpi.ActionMethod);
                            ICommand command = null;

                            //if it has an argument the create a ActionCommand<type> to pass in event argument.. 
                            if (cpi.ActionHasArgument)
                            {
                                Type[] types = { actionType };
                                object[] param = { openDelegate };
                                
                                command = CreateActionCommandWithArguments(cpi.ActionMethodType,types,param);

                            }
                            else
                                command = CreateActionCommand((Action)openDelegate);
                            
                            propVal.Value = command;

                        }
                        else //it's a plain old property.. 
                        {
                            MethodInfo collectionEvent = null;

                            //TODO set collection changed event.. 
                            //if it's a collection or generic collection..
                            if (typeof(System.Collections.ICollection).IsAssignableFrom(property.PropertyType) || property.PropertyType.IsGenericTypeOf(typeof(System.Collections.Generic.ICollection<>)) && linkedOC.TryGetValue(cpi.Name, out collectionEvent))
                            {


                            }

                        }
                PropertyValues.Add(propVal.CustomProperty.Name, propVal);
            }
        }


        /// <summary>
        /// Rollback changes
        /// </summary>
        public void RollBack()
        {
            //loop through each property
            foreach (KeyValuePair<string, PropertyValue> kvp in PropertyValues)
            {
                //if dirty flag then try and rollback value
                if (kvp.Value.IsDirty)
                {

                    if (kvp.Value.CustomProperty.IsPrimitive)
                    {
                        SetPropertyValue(kvp.Value.CustomProperty.Name, kvp.Value.OriginalValue);
                        kvp.Value.IsDirty = false;
                    }
                        else //TODO implement logic to clone back original 
                    {

                    }
                }
            }
            
        }


        public bool IsDirty()
        {

            bool isDirty= false;
            //loop through each property
            foreach (KeyValuePair<string, PropertyValue> kvp in PropertyValues)
            {
                //if dirty flag then try and rollback value
                if (kvp.Value.CustomProperty.IsPrimitive)
                {
                    if (kvp.Value.IsDirty)
                    {
                        isDirty =true;
                        break;
                    }
                }
                else // 
                {
                    //TODO need to check if collection enumerate /object - 
                    if (kvp.Value.CustomProperty.IsObservable)
                    {

                    }
                }
            }
            return isDirty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="ProxyObject"></param>
        public static void AddProxyObject(Type proxyType) 
        {
      
            //add object properties
            foreach (var property in proxyType.GetProperties())
            {

                CustomPropertyInfoHelper cpi = new CustomPropertyInfoHelper(typeof(T), property); 

                cpi.IsProxy = true;
                cpi.ProxyObjectTypeFullName = proxyType.FullName;
                cpi.ProxyObjectType = proxyType;

                Type propertyType = null;

                //
                if (property.PropertyType.IsGenericType &&  Nullable.GetUnderlyingType(property.PropertyType) != null)
                    propertyType = Nullable.GetUnderlyingType(property.PropertyType);
                else
                    propertyType = property.PropertyType;

                //'base type' is considered any primitive types and 'native' .net types such as string and date values
                bool isBaseType = IsPrimitive(propertyType);

                cpi.IsPrimitive = isBaseType;


                //if any collections then create a observable collection for it.. 
                if (!isBaseType )
                {
                    //if it's a collection or generic collection..
                    if (typeof(System.Collections.ICollection).IsAssignableFrom(property.PropertyType) || property.PropertyType.IsGenericTypeOf(typeof(System.Collections.Generic.ICollection<>)))
                    {

                        //create a observable collection of enumerable type..
                        Type virtualObservable = typeof(CustomObservableCollection<>); 

                        //get object type
                        Type modelType = property.PropertyType.GetGenericArguments()[0];

                        Type proxytype = TypeStuff.ProxyToCustomTypes[modelType];
                        cpi.IsObservable = true;

                        cpi.ObservableType = virtualObservable.MakeGenericType(proxytype);
                        cpi.ObservableProxyType = proxytype;
                        cpi.ObservableModelType = modelType;


                        if (linkedOC.ContainsKey(property.Name))
                        {
                            cpi.ObservableCollectionChangedEvent = linkedOC[property.Name];
                        }

                    }
                    else
                    {
                        object Values = null;
                        string Name = string.Empty;

                        //if the type exists in lookup dictiony add property..
                        if (TypeStuff.GetLookupData(property.PropertyType, out Name, out Values))
                        {
                            //assign default value
                            CustomPropertyInfoHelper lookupProperty = AddProperty(Name, typeof(object));
                            lookupProperty.DefaultValue = Values;
      
                        }
                    }
                }

                //TODO null check these - methods may not exist
                cpi.ProxyGetMethod = proxyType.GetMethods().Single(m => m.Name == "get_" + property.Name && !m.IsGenericMethod);
                cpi.ProxySetMethod = proxyType.GetMethod("set_" + property.Name);
                cpi.ProxyPropertyInfo = property;

                try
                {
                    CustomProperties.Add(property.Name, cpi);
                }
                catch (Exception ex)
                {
                    string mess;
                    mess = ex.Message;
                }
                }

            //loop through linked properties 
            foreach (KeyValuePair<string, List<string>> linkedProperty in linkedProperties)
            {
                CustomPropertyInfoHelper cpi = null;

                if (CustomProperties.TryGetValue(linkedProperty.Key, out cpi))
                {
                    cpi.LinkedProperties = linkedProperty.Value;
                }
            }
          
        }

        //'base type' is considered any primitive types and 'native' .net types such as string and date values
        static bool IsPrimitive(Type propertyType)
        {
            return (propertyType.IsPrimitive || propertyType == typeof(Decimal)
               || propertyType == typeof(DateTime) || propertyType == typeof(String)
               || propertyType == typeof(DateTimeOffset) || propertyType == typeof(TimeSpan)
               || propertyType == typeof(Guid));

        }


        /// <summary>
        /// SetValue 
        /// </summary>
        /// <param name="proxyType"></param>
        /// <param name="value"></param>
        public void SetValue(Type proxyType, object value)  //, bool useName = false  || (useName && proxyObject.Value.CustomProperty.IsProxy)
        {
            //get all properties for passed object type
            var proxyObjects = from proxyObject in PropertyValues
                               where (proxyObject.Value.CustomProperty.IsProxy && proxyObject.Value.CustomProperty.ProxyObjectTypeFullName== proxyType.FullName)
                                  select proxyObject.Value;

            ProxyObjects[proxyType.FullName] = value;

            //loop through proxy objet properties and get value..
            foreach (PropertyValue cpi in proxyObjects)
            {

                cpi.ProxyObject = value;
                cpi.Value = cpi.CustomProperty.ProxyPropertyInfo.GetValue(value);
                
                //clone 
                if (cpi.CustomProperty.IsPrimitive)
                    cpi.OriginalValue = cpi.Value;
                else
                    cpi.OriginalValue = cpi.Value;

                cpi.IsDirty = false;

                //if observable collection type create a observable collection, add the values and wire up CollectionChanged event
                if (cpi.CustomProperty.IsObservable )
                {

                    //create  an instance of the custom observable collection
                    dynamic viewModelObservableCollection = cpi.CustomProperty.ObservableType.GetConstructor(Type.EmptyTypes).Invoke(null) as ICustomObservableCollection;
                    
                    viewModelObservableCollection.ProxyType = cpi.CustomProperty.ObservableProxyType;
                    viewModelObservableCollection.ModelType= cpi.CustomProperty.ObservableModelType;
                    viewModelObservableCollection.ProxyObject = cpi.Value;
                    //wire up linked collection changed event

                    //create an instance of model 
                    ICollection iColl = cpi.Value as ICollection;

                    //enumerate structure and add all values to observablecollection
                    IEnumerator iEnumerator= iColl.GetEnumerator();
                    

                    //loop through all values and add to view model
                    while (iEnumerator.MoveNext())
                    {
                        dynamic ddadd = cpi.CustomProperty.ObservableProxyType.GetConstructor(Type.EmptyTypes).Invoke(null);

                        object avalue = iEnumerator.Current;
                        
                        ddadd.SetValue(avalue.GetType(), avalue);
                        ((INotifyPropertyChanged)ddadd).PropertyChanged += _PropertyChanged;
                        viewModelObservableCollection.Add(ddadd);
  
                    }
                    cpi.ObservableObject = viewModelObservableCollection;
                    viewModelObservableCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(oCollectionChanged);
                    viewModelObservableCollection.ProxyObject = cpi.Value;

                    viewModelObservableCollection.CustomCollectionChangedMethod = cpi.CustomProperty.ObservableCollectionChangedEvent;

                   
                }
            }
        }

        /// <summary>
        /// Fires when any proxy ObservableCollection objects changes (additions/deletions).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void oCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ICustomObservableCollection customOC = sender as ICustomObservableCollection;

            //if adding a new item to collection create a new underlying proxy object
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object obj in e.NewItems)
                {
                    //create instance of proxy object
                    object newObject =  customOC.ModelType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    ((ICustomObjectProxyObjects)obj).SetValue(customOC.ModelType,newObject );


                        ((INotifyPropertyChanged)obj).PropertyChanged += _PropertyChanged;


                    //add to underlying proxy collection..
                    ((IList)customOC.ProxyObject).Add(newObject);
                }
            }
            else //remove item from underlying collection
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                //loop through and remove from proxy object
                foreach (ICustomObjectProxyObjects obj in e.OldItems)
                {
                    ((INotifyPropertyChanged)obj).PropertyChanged -= _PropertyChanged;

                    ((IList)customOC.ProxyObject).Remove(obj.GetProxyObjects().Values.ToList()[0]);
                }
            }

            //is a custom collection change medthod set? Call it - pass through parameters
            if (customOC.CustomCollectionChangedMethod != null)
            {
                customOC.CustomCollectionChangedMethod.Invoke(this, new object[2] { sender, e });
            }

        }

        /// <summary>
        /// Adds a new property definition for a string type
        /// </summary>
        /// <param name="name"></param>
        public static CustomPropertyInfoHelper AddProperty(string name)
        {
            return AddProperty(name, typeof(string));
        }

        /// <summary>
        /// Clears all the properties from a defined object type.
        /// </summary>
        public static void ClearProperties()
        {
            //TODO eventually remove CustomProperties
            CustomProperties.Clear();
        }

        /// <summary>
        /// Removes a specific property by name
        /// </summary>
        /// <param name="name">Property to remove</param>
        public static void RemoveProperty(string name)
        {

            if (CustomProperties.ContainsKey(name))
                CustomProperties.Remove(name);
        }

        /// <summary>
        /// Adds a new dynamic property value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        public static CustomPropertyInfoHelper  AddProperty(string name, Type propertyType, object value = null)
        {
            CustomPropertyInfoHelper newProperty = null;
            if (!CustomProperties.ContainsKey(name))
            {
                newProperty = new CustomPropertyInfoHelper(name, propertyType, typeof(T));

                if (value != null)
                    newProperty.DefaultValue = value;

                newProperty.IsPrimitive = IsPrimitive(newProperty.PropertyType);
                CustomProperties.Add(name, newProperty);
            }

            return newProperty;
        }

        /// <summary>
        /// Sets a property value by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(string propertyName, object value)
        {
            PropertyValue propVal;

            if ( PropertyValues.TryGetValue(propertyName,out propVal ) )
            {
                CustomPropertyInfoHelper propertyInfo = propVal.CustomProperty;

                //validate property type against value being set
                if (ValidateValueType(value, propertyInfo.PropertyType))  
                {

                    //if value changed then set property and raise events..
                    if (PropertyValues[propertyName].Value != value)
                    {
                        //if proxy/wrapped object set the source object's property 
                        if (propVal.CustomProperty.IsProxy)
                        {
                            //if proxy object not set then create new instance
                            if (propVal.ProxyObject == null)
                            {
                                propVal.ProxyObject = propertyInfo.ProxyObjectType.GetConstructor(Type.EmptyTypes).Invoke(null);  

                                SetValue(propertyInfo.ProxyObjectType, propVal.ProxyObject); 

                            }
                            else

                            //call the setter on proxy object
                            propertyInfo.ProxyPropertyInfo.SetValue(propVal.ProxyObject, value); //propertyInfo.

                            //if it's a local property then read the set value back.
                            if (propVal.CustomProperty.IsLocalProxy)
                                value = propertyInfo.ProxyPropertyInfo.GetValue(propVal.ProxyObject);
                        }

                        if (!propVal.IsDirty && propVal.OriginalValue == null)
                            propVal.OriginalValue = value;

                        propVal.IsDirty = true;
                        propVal.Value = value;
                        if (propVal.CustomProperty.RaisePropertyChangedEvent)
                            RaisePropertyChanged(propertyName);

                        //do we need to fire any linked properties? 
                        if (propVal.CustomProperty.LinkedProperties != null)
                        {
                            foreach (string propertyNam in propVal.CustomProperty.LinkedProperties)
                            {
                                if (propVal.CustomProperty.RaisePropertyChangedEvent)
                                    RaisePropertyChanged(propertyNam);
                            }
                        }

                    }
            }             
            else 
                throw new Exception("Value is of the wrong type or null for a non-nullable type.");
            }
            else
                throw new Exception("There is no property with the name " + propertyName); 
            }

        /// <summary>
        /// Returns a specific property value by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            PropertyValue propVal;

            if (PropertyValues.TryGetValue(propertyName, out propVal))
            {
                CustomPropertyInfoHelper propertyInfo = propVal.CustomProperty;

                if (propertyInfo.IsProxy)
                {
                    //if no setter method then execute get method..
                    if (propertyInfo.ProxySetMethod == null)
                    {
                        propVal.Value = propertyInfo.ProxyPropertyInfo.GetValue(propVal.ProxyObject);

                    }
                    else
                        propVal.Value = propertyInfo.ProxyPropertyInfo.GetValue(propVal.ProxyObject);
                }
                return propertyInfo.IsObservable ? propVal.ObservableObject : propVal.Value;

            }
            else
                throw new Exception("There is no property " + propertyName); 
        }

        /// <summary>
        /// Returns a cast property value by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public TV GetPropertyValue<TV>(string propertyName)
        {
            return (TV) GetPropertyValue(propertyName);
        }

        /// <summary>
        /// Retrieve all the defined properties (both known and dynamic)
        /// </summary>
        /// <returns></returns>
        public PropertyInfo[] GetProperties()
        {
            return GetCustomType().GetProperties();
        }

        /// <summary>
        /// Gets the custom type provided by this object.
        /// </summary>
        /// <returns>
        /// The custom type. 
        /// </returns>
        public Type GetCustomType()
        {
            return _ctype.Value;
        }

        /// <summary>
        /// Validates the value for the given type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool ValidateValueType(object value, Type type)
        {
            return value == null
                ? !type.IsValueType || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                : type.IsAssignableFrom(value.GetType());
        }


        public Dictionary<string, object> GetProxyObjects()
        {
            return ProxyObjects;
        }
        #region Data
        private static readonly Dictionary<string,CustomPropertyInfoHelper> CustomProperties = new Dictionary<string,CustomPropertyInfoHelper>();
         
        public readonly Dictionary<string, object> ProxyObjects; // {get;set;} 
        public readonly Dictionary<string, PropertyValue> PropertyValues = new Dictionary<string, PropertyValue>();
        
        private readonly Lazy<CustomType> _ctype = new Lazy<CustomType>(() => new CustomType(typeof(T)));
        #endregion
    }

    public class PropertyValue
    {
        public object Value { get; set; }
        public CustomPropertyInfoHelper CustomProperty { get; set; }
        object proxyObject;


        public object ProxyObject
        {
            get { return proxyObject; }
            set
            {
                if (proxyObject != value)
                    proxyObject = value;
            }
        }
        public object ObservableObject { get; set; }
        public string Name { get; set; }
        public object OriginalValue { get; set; }
        public bool IsDirty { get; set; }

        public PropertyValue(object Value, CustomPropertyInfoHelper cpi)
        {
            this.OriginalValue = Value;
            this.Value = Value;
            CustomProperty = cpi;
        }

        public override string ToString()
        {
            return String.Format( "Dirty:{0}  {1}", IsDirty, Value.ToString());
        }

    }

}
