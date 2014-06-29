using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace ProxyHelper
{
    public class ViewModelBase<T> : ProxyTypeHelper<T>
    {
    }

    public class ViewModelValidated<T> : ProxyTypeHelper<T>, IDataErrorInfo, IIsValid
    {
        [NotMapped]
        public string Error
        {
            get
            {
                return this.error;
            }
        }

        private string error = string.Empty;

        public string this[string columnName]
        {
            get
            {

                //if validate property 
                if (this.ValidateProperty(columnName, ref this.error)) 
                {
                    //get the column name being validated..
                    PropertyValue propV = this.PropertyValues[columnName];

                    //if it's a proxy object then try and validate the underlying entites data annotations..
                    if (propV.CustomProperty.IsProxy)
                    {
                        ValidationContext validationContext = new ValidationContext(propV.ProxyObject, null, null) { MemberName = columnName };
                        List<ValidationResult> validationResults = new List<ValidationResult>();
                        if (!Validator.TryValidateProperty(propV.Value, validationContext, validationResults))
                        {
                            error = validationResults[0].ErrorMessage;

                        }
                        else
                            error = "";
                    }
                }
                return this.error;
            }
        }


        public virtual  bool IsValid()
        {
            //get the whole proxy entity object to validate
            var data = ProxyObjects[TypeStuff.CustomTypesToProxy[typeof(T)].ToString()];

            ValidationContext validationContext = new ValidationContext(data, null, null);

            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(data,validationContext,validationResults ,true))
            {
                return false;
            }

            return true; 
        }

        //override this in inherited class to provide additional validation
        public virtual bool ValidateProperty(string propertyName, ref string error)
        {
            return true;
        }





    }
}
