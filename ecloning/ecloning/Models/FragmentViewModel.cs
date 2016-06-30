using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class FragmentViewModel
    {
        public int id { get; set; }
        public int plasmid_id { get; set; }
        public string fName { get; set; }
        public List<string> enzymes { get; set; }
        public int f_start { get; set; }        
        public int f_end { get; set; }
        public string fSeq { get; set; }
        public List<int> overhangs { get; set; }
        public string cSeq { get; set; }

        //feature array
        public IList<fragmentFeatures> featureArray { get; set; }
    }
}