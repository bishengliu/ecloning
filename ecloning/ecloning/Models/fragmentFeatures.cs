using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class fragmentFeatures
    {
        public bool clockwise { get; set; }
        public int? cut { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string feature { get; set; }
        public int show_feature { get; set; }
        public int type_id { get; set; }
    }

    public class fragmentJson
    {
        public IList<fragmentFeatures> featureArray { get; set; }
        public int bandStart { get; set; }
        public int bandEnd { get; set; }
        public string fSeq { get; set; }
        public string cSeq { get; set; }
        public int ladder_id { get; set; }
        public bool parental { get; set; }
        public int plasmid_id { get; set; }
        public List<int> overhangs { get; set; }
        public List<string> enzymes { get; set; }
    }
}