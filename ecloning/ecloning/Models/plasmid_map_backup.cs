//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ecloning.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class plasmid_map_backup
    {
        public int id { get; set; }
        public int plasmid_id { get; set; }
        public int show_feature { get; set; }
        public string feature { get; set; }
        public int feature_id { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public Nullable<int> cut { get; set; }
        public Nullable<int> common_id { get; set; }
        public int clockwise { get; set; }
        public string des { get; set; }
    
        public virtual common_feature common_feature { get; set; }
        public virtual plasmid plasmid { get; set; }
        public virtual plasmid_feature plasmid_feature { get; set; }
    }
}
