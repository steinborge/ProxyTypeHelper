// based on http://blogs.msdn.com/b/silverlight_sdk/archive/2011/04/25/binding-to-dynamic-properties-with-icustomtypeprovider-silverlight-5-beta.aspxhttp://blogs.msdn.com/b/silverlight_sdk/archive/2011/04/25/binding-to-dynamic-properties-with-icustomtypeprovider-silverlight-5-beta.aspx
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProxyHelper
{
    // Custom implementation of the PropertyInfo. Stores extended information on custom proxy objects
    public class CustomPropertyInfoHelper : PropertyInfo
    {
        readonly string _name;
        internal readonly Type _type;
        readonly MethodInfo _getMethod, _setMethod;
        readonly List<Attribute> _attributes = new List<Attribute>();
         
        //proxy objects - 
        public bool IsProxy { get; set; }
        public bool IsLocalProxy { get; set; }

        public bool IsPrimitive { get; set; }
        public bool IsObservable{ get; set; }
        public Type ObservableType { get; set; }
        public Type ObservableProxyType { get; set; }
        public Type ObservableModelType { get; set; }
        public MethodInfo  ObservableCollectionChangedEvent  { get; set; }
        public object ObservableProxyValue { get; set; }



        public bool RaisePropertyChangedEvent { get; set; }


        public bool IsAction { get; set; }
        public bool ActionHasArgument { get; set; }
        public Type ActionArgumentType { get; set; }
        public Type ActionMethodType { get; set; }
        public MethodInfo ActionMethod { get; set; }


        public string ProxyObjectTypeFullName { get; set; }
        public Type ProxyObjectType { get; set; }

        public PropertyInfo ProxyPropertyInfo { get; set; }
        public MethodInfo ProxyGetMethod { get; set; }
        public MethodInfo ProxySetMethod { get; set; }

        public object DefaultValue { get; set; }
        public List<string> LinkedProperties { get; set; }


        readonly MethodInfo _proxyGetMethod, _proxySetMethod;

        public CustomPropertyInfoHelper(string name, Type type, Type ownerType)
        {
            RaisePropertyChangedEvent = true;
            _name = name;
            _type = type;

            MethodInfo[] dd = ownerType.GetMethods();
            //set get/set methods to point to custom property
            _getMethod = ownerType.GetMethods().Single(m => m.Name == "GetPropertyValue" && !m.IsGenericMethod);
            _setMethod = ownerType.GetMethod("SetPropertyValue");
        }
        
        public CustomPropertyInfoHelper(string name, Type type, List<Attribute> attributes, Type propertyOwner)
        {
            RaisePropertyChangedEvent = true;
            _name = name;
            _type = type;
            _attributes = attributes;
        }

        public CustomPropertyInfoHelper(Type ownerType, PropertyInfo proxyPinfo, bool NotMapped = false)
        {
            RaisePropertyChangedEvent = true;

            _name = proxyPinfo.Name; // name;
            _type = proxyPinfo.PropertyType;

            if (NotMapped)
            {
                _getMethod = ownerType.GetMethods().Single(m => m.Name == "get_" + proxyPinfo.Name && !m.IsGenericMethod);
                _setMethod = ownerType.GetMethod("set_" + proxyPinfo.Name);
            }
            else
            {
                _getMethod = ownerType.GetMethods().Single(m => m.Name == "GetPropertyValue" && !m.IsGenericMethod);
                _setMethod = ownerType.GetMethod("SetPropertyValue");
            }
            ProxyPropertyInfo = proxyPinfo;

             _proxySetMethod = proxyPinfo.GetSetMethod();
             _proxyGetMethod = proxyPinfo.GetGetMethod();
        }


        public override PropertyAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return _getMethod;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return _setMethod;
        }

        // Returns the value from the dictionary stored in the Customer's instance.
        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            return _getMethod.Invoke(obj, new object[] { _name });
        }

        public override Type PropertyType
        {
            get { return _type; }
        }

        // Sets the value in the dictionary stored in the Customer's instance.
        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            _setMethod.Invoke(obj, new[] { _name, value });
            obj.GetType().GetMethod("SetPropertyValue").Invoke(obj, new[] { _name, value });
        }

        public override Type DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            var attrs = from a in _attributes where a.GetType() == attributeType select a;
            return attrs.ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _attributes.ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return _name; }
        }

        public override Type ReflectedType
        {
            get { throw new NotImplementedException(); }
        }

        internal List<Attribute> CustomAttributesInternal
        {
            get { return _attributes; }
        }
    }
}