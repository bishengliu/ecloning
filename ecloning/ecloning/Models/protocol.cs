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
    
    public partial class protocol
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public protocol()
        {
            this.exp_step = new HashSet<exp_step>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> version { get; set; }
        public Nullable<int> versionref { get; set; }
        public Nullable<int> people_id { get; set; }
        public string des { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public Nullable<int> type_id { get; set; }
        public string upload { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<exp_step> exp_step { get; set; }
        public virtual person person { get; set; }
    }
}
