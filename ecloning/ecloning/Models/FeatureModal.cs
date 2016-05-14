using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class FeatureModal
    {
        //this is the modal when selecting plasmid seq to quickly add to common features or primer
        //for common feature or primer
        public int feature_id { get; set; } //discard when primer
        public string label { get; set; } //= primer name
        public int sequence { get; set; } //=primer seq
        //public int group_id { get; set; } //not used for primer
        //public int people_id { get; set; } //also for primer

        //add feature in plasmid map
        public int plasmid_id { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }
}