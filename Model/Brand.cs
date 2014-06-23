using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Model
{
    [Table("Brand")]
    public partial class Brand  
    {
 
        [Display(Name = "Brand ID")][Required][Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BrandID")] 
        public  Int64 BrandID {get;set;}

        [Display(Name = "Brand Name")][MaxLength(50)]
        [Column("BrandName")] 
        public  string BrandName {get;set;}

    }



}
