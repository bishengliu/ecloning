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
    
    public partial class group_shared
    {
        public int id { get; set; }
        public int group_id { get; set; }
        public int resource_id { get; set; }
        public string category { get; set; }
        public string sratus { get; set; }
    
        public virtual group group { get; set; }
    }
}
