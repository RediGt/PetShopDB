using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSQL
{
    class Pets
    {
        public string id;
        public string type;
        public string description;
        public string name;
        public string status;
        public string staffFirstName;
        public string staffLastName;
        public string clientFirstName;
        public string clientLastName;

        public Pets(string id, string type, string description, string name, 
                    string status, string staffFirstName, string staffLastName,
                    string clientFirstName, string clientLastName)
        {
            this.id = id;
            this.type = type;
            this.description = description;
            this.name = name;
            this.status = status;
            this.staffFirstName = staffFirstName;
            this.staffLastName = staffLastName;
            this.clientFirstName = clientFirstName;
            this.clientLastName = clientLastName;
        }
    }
}
