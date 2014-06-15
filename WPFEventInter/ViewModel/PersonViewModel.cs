using System;
using System.Windows;
using System.Windows.Controls;
using ProxyHelper;
using Model;


namespace WPFEventInter
{
    public class PersonViewModel : ViewModelValidated<PersonViewModel> 
    {

        public PersonViewModel()
        {
            //assign some initial property values
           SetPropertyValue("Name", "Bob");
           SetPropertyValue("LastName", "Jones");
        
        }


        /// <summary>
        /// demonstrating class property - note no property change event fired
        /// </summary>
        string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value.ToUpper(); }
        }

        //demonstrate auto wiring of observable colletion changed event..
        [LinkToCollection("ContactDetails")]
        public void ContactDetailsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object obj in e.NewItems)
                {
                    ((ContactViewModel)obj).SetPropertyValue("Detail", "new contact detail"); 
                }
            }
        }


        /// <summary>
        /// Full name shows combined first and surname properties. The LinkToProperty ensures property is fired when changes are made
        /// </summary>
        [LinkToProperty("FirstName")]
        [LinkToProperty("SurName")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", GetPropertyValue<string>("FirstName"), GetPropertyValue<string>("SurName"));
            }
        }

        /// <summary>
        /// Demonstrates wiring view events - this wires to data grid view AddingNewItem event
        /// </summary>
        /// <param name="e"></param>
        [LinkToEvent("AddingNewItemEvent")]
        private void DataGrid_AddingNewItem(AddingNewItemEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        [LinkToCommand("RollBackChangesCommand")]
        private void RollBackChanges()
        {
            this.RollBack();
        }
        

        /// <summary>
        /// Wire up all propery change events through attribute. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [PropertyChangedEvent]
        void person_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName != "Log" && sender is ICustomObjectProxyObjects)
            {
                SetPropertyValue("Log", e.PropertyName + " changed to:" + ((ICustomObjectProxyObjects)sender).GetPropertyValue(e.PropertyName).ToString() + "\r\n" + Log);
            }
        }


        public string Log { get; set; }
 
    }

    [ProxyHelper.AssociatedModel(typeof(ContactDetail))]
    public class ContactViewModel : ViewModelBase<ContactViewModel>
    {
    }


}
