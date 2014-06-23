using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Model
{
    [Table("CarType")]
    public partial class CarType  
    {
 
        [Display(Name = "Car Type ID")][Required][Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CarTypeID")] 
        public  Int64 CarTypeID {get;set;}

        [Display(Name = "Car Type Name")][MaxLength(50)]
        [Column("CarTypeName")] 
        public  string CarTypeName {get;set;}
    }


}
