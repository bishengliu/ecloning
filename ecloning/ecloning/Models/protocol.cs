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
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string purpose { get; set; }
        public Nullable<int> version { get; set; }
        public Nullable<int> versionref { get; set; }
        public Nullable<int> people_id { get; set; }
        public string des { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public Nullable<bool> submitted_to_group { get; set; }
        public Nullable<bool> shared_with_group { get; set; }
        public string shared_with_people { get; set; }
    
        public virtual person person { get; set; }
    }
}