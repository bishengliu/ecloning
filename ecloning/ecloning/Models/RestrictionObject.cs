using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RestrictionObject
    {
        public string name { get; set; }
        public string prototype { get; set; }
        public string startActivity { get; set; }
        public string heatInactivation { get; set; }
        public string dam { get; set; }
        public string dcm { get; set; }
        public string cpg { get; set; }
        public int forward_cut { get; set; }
        public int reverse_cut { get; set; }
        public int? forward_cut2 { get; set; }
        public int? reverse_cut2 { get; set; }
    }
}