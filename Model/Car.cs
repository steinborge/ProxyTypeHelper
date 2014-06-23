using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Model
{
    [Table("Car")]
    public partial class Car  
    {
 
        [Display(Name = "Car ID")][Required][Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CarID")] 
        public  Int64 CarID {get;set;}

        [Required (ErrorMessage="Car name is required")]
        [Display(Name = "Car Name")]
        [MaxLength(50, ErrorMessage = "Car name cannot be longer than 50 characters")]
        [Column("CarName")] 
        public  string CarName {get;set;}

        [Display(Name = "Colour ID")]
        [Column("ColourID")] 
        public  System.Nullable<Int64> ColourID {get;set;}

        [Display(Name = "Car Type ID")]
        [Column("CarTypeID")] 
        public  System.Nullable<Int64> CarTypeID {get;set;}

        [Display(Name = "Brand ID")]
        [Column("BrandID")] 
        public  System.Nullable<Int64> BrandID {get;set;}

        [Display(Name = "Release Date")]
        [Column("ReleaseDate")] 
        public  System.Nullable<DateTime> ReleaseDate {get;set;}

        [Range (1,100000)]
        [Display(Name = "Price")]
        [Column("Price")] 
        public  Decimal Price {get;set;}


        [ForeignKey("ColourID")]
        public virtual Colour Colour {get;set;}

        [ForeignKey("CarTypeID")]
        public virtual CarType CarType {get;set;}

        [ForeignKey("BrandID")]
        public virtual Brand Brand {get;set;}
        }

}
