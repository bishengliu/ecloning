using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class RestrictionActivity
    {        
        public int company_id { get; set; }
        public int enzyme_id { get; set; }
        public int temprature { get; set; }

        //buffer name and activity
        public IList<Dictionary<int, int>>Activity { get; set; }
        //buffer id and activity in %
        //activity: 0 is <10%, 1 is 10%, 2 is 10-25%, 3 is 25%,  4 is 25%-50%, 5 is 50%, 6 is 50-75%,  7 is 75%, 8 is 75-100%, 9 is 100%
        //proprtery
        public bool? starActivity { get; set; }
        public int? inactivity { get; set; }
        public bool? dam { get; set; }
        public bool? dcm { get; set; }
        public bool? cpg { get; set; }
    }
}