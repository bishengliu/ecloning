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
    
    public partial class fragment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public fragment()
        {
            this.exp_step_material = new HashSet<exp_step_material>();
            this.exp_step_material1 = new HashSet<exp_step_material>();
            this.fragment_map = new HashSet<fragment_map>();
            this.fragment_methylation = new HashSet<fragment_methylation>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> plasmid_id { get; set; }
        public Nullable<int> fragment_id { get; set; }
        public bool parantal { get; set; }
        public string enzyme_id { get; set; }
        public string company_id { get; set; }
        public string buffer_id { get; set; }
        public Nullable<int> forward_start { get; set; }
        public Nullable<int> forward_end { get; set; }
        public Nullable<int> forward_size { get; set; }
        public string forward_seq { get; set; }
        public Nullable<int> rc_start { get; set; }
        public Nullable<int> rc_end { get; set; }
        public Nullable<int> rc_size { get; set; }
        public string rc_seq { get; set; }
        public Nullable<int> rc_left_overhand { get; set; }
        public Nullable<int> rc_right_overhand { get; set; }
        public Nullable<int> ladder_id { get; set; }
        public int people_id { get; set; }
        public System.DateTime dt { get; set; }
        public string des { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<exp_step_material> exp_step_material { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<exp_step_material> exp_step_material1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fragment_map> fragment_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fragment_methylation> fragment_methylation { get; set; }
        public virtual person person { get; set; }
        public virtual plasmid plasmid { get; set; }
    }
}
