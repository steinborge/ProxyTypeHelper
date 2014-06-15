using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyHelper
{

    public class PropertyChangedEvent: System.Attribute
    {

    }

    public interface INamedAttribute
    {
        String Name { get ;  }
    }


    public class LinkToCommand : System.Attribute, INamedAttribute
    {
        private string actionName;
        private bool passArgument;

        public String Name { get { return actionName; } }
        public bool PassArgument { get { return passArgument; } }


        public LinkToCommand(string ActionName)
        {
            actionName = ActionName;

        }
        public LinkToCommand(string ActionName, bool PassArgument)
        {
            actionName = ActionName;
            passArgument = PassArgument;
        }

    }

    public class LinkToEvent : System.Attribute, INamedAttribute
    {
        private string eventName;
        public String Name { get { return eventName; } }

        public LinkToEvent (string EventName)
        {
            eventName = EventName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property,AllowMultiple = true)  ]
    public class LinkToProperty : System.Attribute, INamedAttribute
    {
        private string propertyName;
        public String Name { get { return propertyName; } }

        public LinkToProperty(string PropertyName)
        {
            propertyName = PropertyName;
        }
    }


    public class LinkToCollection : System.Attribute, INamedAttribute
    {
        private string collectionName;
        public String Name { get { return collectionName; } }

        public LinkToCollection(string PropertyName)
        {
            collectionName = PropertyName;
        }
    }


    public class NotMapped : System.Attribute
    {
    }

    public class DoNotRaiseProperyChangedEvents : System.Attribute
    {
    }


    public class AssociatedModel : System.Attribute
    {
        private Type proxyType;
        public Type ProxyType { get { return proxyType; } }
        public bool RaiseProperyChangedEvents { get; set; }

        public AssociatedModel( Type ProxyType, bool DoNotRaisePropertyChangedEvents = false)
        {
            proxyType = ProxyType;
            RaiseProperyChangedEvents = DoNotRaisePropertyChangedEvents;
        }
    }

}
