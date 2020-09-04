using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tr.Data
{
    public class Base
    {
        public class JsonError
        {
            public string operation { get; set; }
            public string error { get; set; }
            public string error_description { get; set; }
        }
    }
}
