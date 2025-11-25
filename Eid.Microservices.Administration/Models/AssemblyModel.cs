using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Administration.Models
{
    public class AssemblyModel
    {
        public string Parent { get; set; }
        public List<string> Members { get; set; }
    }
}
