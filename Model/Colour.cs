using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Model
{
    [Table("Colour")]
    public partial class Colour  
    {
 
        [Display(Name = "Colour ID")][Required][Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ColourID")] 
        public  Int64 ColourID {get;set;}

        [Display(Name = "Colour Name")][MaxLength(50)]
        [Column("ColourName")] 
        public  string ColourName {get;set;}


    }

}
