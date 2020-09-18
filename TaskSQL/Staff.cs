using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSQL
{
    class Staff
    {
        public string id;
        public string staffFirstName;
        public string staffLastName;
        public string staffPhone;

        public Staff(string id, string staffFirstName, string staffLastName, string staffPhone)
        {
            this.id = id;            
            this.staffFirstName = staffFirstName;
            this.staffLastName = staffLastName;
            this.staffPhone = staffPhone;
        }
    }
}
