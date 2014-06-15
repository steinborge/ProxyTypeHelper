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
    static public class TypeStuff
    {
        public class LookupTypeInfo
        {
            public string Name { get; set; }
            public object Value { get; set; }

            public LookupTypeInfo(string Name, object Value)
            {
                this.Name = Name;
                this.Value = Value;
            }
        }

        static Dictionary<Type, LookupTypeInfo> LookupValueTypes = new Dictionary<Type, LookupTypeInfo>();

        public static Dictionary<Type, Type> ProxyToCustomTypes = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> CustomTypesToProxy = new Dictionary<Type, Type>();

        public static void SetLookupData(string Name, Type DataType, object Values)
        {
            LookupValueTypes.Add(DataType, new LookupTypeInfo(Name, Values));
        }


        public static bool GetLookupData(Type DataType, out string Name, out object Values)
        {
            LookupTypeInfo lookupInfo;

            Name = string.Empty;
            Values = null;

            if (LookupValueTypes.TryGetValue(DataType, out lookupInfo))
            {
                Name = lookupInfo.Name;
                Values = lookupInfo.Value;

                return true;
            }
            return false;
        }

        /// <summary>
        /// Associate any models with view models
        /// </summary>
        public static void InitializeProxyTypes()
        {
            //loop through all alssemblies
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (Type type in assembly.GetTypes())
                {
                        object[] customAttributes = type.GetCustomAttributes(typeof(AssociatedModel), true);

                        if (customAttributes != null && customAttributes.Length > 0)
                        {
                            AssociatedModel assocClass = customAttributes[0] as AssociatedModel;

                            ProxyToCustomTypes.Add(assocClass.ProxyType, type);
                            CustomTypesToProxy.Add(type, assocClass.ProxyType);
                        }
                }

            //assign proxy object to class
            foreach (KeyValuePair<Type, Type> kvp in CustomTypesToProxy)
            {
                MethodInfo mi = kvp.Key.GetMethod("AddProxyObject", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);//.Invoke(null, null); 
                mi.Invoke(null, new object[] { kvp.Value });
            }
        }


        public static bool IsGenericTypeOf(this Type t, Type genericDefinition)
        {
            Type[] parameters = null;
            return IsGenericTypeOf(t, genericDefinition, out parameters);
        }


        public static bool IsGenericTypeOf(this Type t, Type genericDefinition, out Type[] genericParameters)
        {
            genericParameters = new Type[] { };
            if (!genericDefinition.IsGenericType)
            {
                return false;
            }

            var isMatch = t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinition.GetGenericTypeDefinition();
            if (!isMatch && t.BaseType != null)
            {
                isMatch = IsGenericTypeOf(t.BaseType, genericDefinition, out genericParameters);
            }
            if (!isMatch && genericDefinition.IsInterface && t.GetInterfaces().Any())
            {
                foreach (var i in t.GetInterfaces())
                {
                    if (i.IsGenericTypeOf(genericDefinition, out genericParameters))
                    {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch && !genericParameters.Any())
            {
                genericParameters = t.GetGenericArguments();
            }
            return isMatch;
        }

    }
}
