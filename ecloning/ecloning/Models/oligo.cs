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
    
    public partial class oligo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string sequence { get; set; }
        public string company { get; set; }
        public string orderref { get; set; }
        public string location { get; set; }
        public string usage { get; set; }
        public string purity { get; set; }
        public string modification { get; set; }
        public Nullable<int> people_id { get; set; }
        public string des { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
    
        public virtual person person { get; set; }
    }
}
