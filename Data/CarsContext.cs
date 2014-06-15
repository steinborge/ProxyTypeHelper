using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using System.Data.Entity;
using Model;
using System.Data;

namespace Data
{
    public partial class CarsContext : System.Data.Entity.DbContext
    {
        public DbSet<Model.Car> Cars { get; set; }
        public DbSet<Model.Brand> Brands { get; set; }
        public DbSet<Model.CarType> CarTypes { get; set; }
        public DbSet<Model.Colour> Colours { get; set; }
    }
}
