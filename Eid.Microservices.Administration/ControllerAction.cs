using System;
using System.Collections.Generic;
using System.Text;

namespace Eid.Microservices.Administration
{
    public class ControllerAction
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ReturnType { get; set; }
        public string Attributes { get; set; }
    }
}
