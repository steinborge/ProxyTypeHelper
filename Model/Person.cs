using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    public class Person
    {
        public List<ContactDetail> ContactDetails { get; set; }

        public string FirstName { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Height { get; set; }

    }

    public class ContactDetail
    {
        public int ID { get; set; }
        public int ContactType { get; set; }
        public string Detail { get; set; }

        public ContactDetail(int ID, string Detail)
        {
            this.ID = ID;
            this.Detail = Detail;

        }

        public ContactDetail()
        {
        }
    }

}
