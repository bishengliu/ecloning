using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RestriFeatureObject
    {
        public int start { get; set; }
        public int end { get; set; }
        public int cut { get; set; }
        public string name { get; set; }
        public int clockwise { get; set; }
        public bool dam_complete { get; set; }
        public bool dam_impaired { get; set; }
        public bool dcm_complete { get; set; }
        public bool dcm_impaired { get; set; }
    }
}