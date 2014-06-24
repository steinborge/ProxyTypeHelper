using System;
using System.Windows.Interactivity;
using System.Windows.Input;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;

namespace Core.WPF.Infrastructure
{
    // <summary>
    //
    /// </summary>
    public class EventToCommandBehavior : Behavior<FrameworkElement>
    {
        private Delegate _handler;
        private EventInfo _oldEvent;

        // Event
        public string Event { get { return (string)GetValue(EventProperty); } set { SetValue(EventProperty, value); } }
        public static readonly DependencyProperty EventProperty = DependencyProperty.Register("Event", typeof(string), typeof(EventToCommandBehavior), new PropertyMetadata(null, OnEventChanged));

        // Command
        public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        // PassArguments (default: false)
        public bool PassArguments { get { return (bool)GetValue(PassArgumentsProperty); } set { SetValue(PassArgumentsProperty, value); } }
        public static readonly DependencyProperty PassArgumentsProperty = DependencyProperty.Register("PassArguments", typeof(bool), typeof(EventToCommandBehavior), new PropertyMetadata(false));


        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var beh = (EventToCommandBehavior)d;

            if (beh.AssociatedObject != null) // is not yet attached at initial load
                beh.AttachHandler((string)e.NewValue);
        }

        protected override void OnAttached()
        {
            AttachHandler(this.Event); 
        }

        /// <summary>
        /// Attaches the handler to the event
        /// </summary>
        private void AttachHandler(string eventName)
        {
            // detach old event
            if (_oldEvent != null)
                _oldEvent.RemoveEventHandler(this.AssociatedObject, _handler);

            // attach new event
            if (!string.IsNullOrEmpty(eventName))
            {
                EventInfo ei = this.AssociatedObject.GetType().GetEvent(eventName);
                if (ei != null)
                {
                    MethodInfo mi = this.GetType().GetMethod("ExecuteCommand", BindingFlags.Instance | BindingFlags.NonPublic);
                    _handler = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                    ei.AddEventHandler(this.AssociatedObject, _handler);
                    _oldEvent = ei; // store to detach in case the Event property changes
                }
                else
                    throw new ArgumentException(string.Format("The event '{0}' was not found on type '{1}'", eventName, this.AssociatedObject.GetType().Name));
            }
        }

        /// <summary>
        /// Executes the Command
        /// </summary>
        private void ExecuteCommand(object sender, EventArgs e)
        {
            object parameter = this.PassArguments ? e : null;
            if (this.Command != null)
            {
                if (this.Command.CanExecute(parameter))
                    this.Command.Execute(parameter);
            }
        }
    }


    public class ActionCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> _action;

        public ActionCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                var castParameter = (T)Convert.ChangeType(parameter, typeof(T));
                _action(castParameter);
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ContentControlGenericTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //TODO remove this later
            //object view = GetRoot(((FrameworkElement)container).Parent);
            DataTemplate retVal = null;

            try
            {
                retVal = Core.WPF.Infrastructure.DataTemplateManager.templates[item.GetType().ToString()];
            }
            catch //empty catch to prevent design time errors..
            {
            }

            return retVal;
        }

        /// <summary>
        /// Get the form root object 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        object GetRoot(DependencyObject parent)
        {

            object retVal; 
            if (((FrameworkElement)parent).Parent != null)
                retVal = GetRoot(((FrameworkElement)parent).Parent);
            else
                return parent;

            return retVal;
        }
    }

}
