using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSQL
{
    class PetType
    {
        public string typeId;
        public string type;
        public string description;

        public PetType(string typeId, string type, string description)
        {
            this.typeId = typeId;
            this.type = type;
            this.description = description;
        }
    }
}
