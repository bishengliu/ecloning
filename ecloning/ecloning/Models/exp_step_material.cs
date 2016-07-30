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
    
    public partial class exp_step_material
    {
        public int id { get; set; }
        public int exp_id { get; set; }
        public int exp_step_id { get; set; }
        public Nullable<int> forward_primer_id { get; set; }
        public Nullable<int> reverse_primer_id { get; set; }
        public Nullable<int> probe_id { get; set; }
        public string emzyme_id { get; set; }
        public Nullable<int> plasmid_id { get; set; }
        public Nullable<int> frag1_id { get; set; }
        public Nullable<int> frag2_id { get; set; }
        public string des { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public Nullable<int> ligation_method { get; set; }
        public Nullable<int> ligation_direction { get; set; }
    
        public virtual experiment experiment { get; set; }
        public virtual primer primer { get; set; }
        public virtual fragment fragment { get; set; }
        public virtual fragment fragment1 { get; set; }
        public virtual plasmid plasmid { get; set; }
        public virtual probe probe { get; set; }
        public virtual primer primer1 { get; set; }
    }
}
