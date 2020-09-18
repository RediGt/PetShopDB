using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSQL
{
    class Clients
    {
        public string clientId;
        public string clientFirstName;
        public string clientLastName;
        public string clientPhone;

        public Clients(string id, string clientFirstName, string clientLastName, string clientPhone)
        {
            this.clientId = id;
            this.clientFirstName = clientFirstName;
            this.clientLastName = clientLastName;
            this.clientPhone = clientPhone;
        }
    }
}
