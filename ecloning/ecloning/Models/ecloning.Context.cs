﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ecloningEntities : DbContext
    {
        public ecloningEntities()
            : base("name=ecloningEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<activity_modifying> activity_modifying { get; set; }
        public virtual DbSet<activity_restriction> activity_restriction { get; set; }
        public virtual DbSet<app_license> app_license { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<buffer> buffers { get; set; }
        public virtual DbSet<common_feature> common_feature { get; set; }
        public virtual DbSet<common_modifying> common_modifying { get; set; }
        public virtual DbSet<common_restriction> common_restriction { get; set; }
        public virtual DbSet<company> companies { get; set; }
        public virtual DbSet<Dam> Dams { get; set; }
        public virtual DbSet<Dcm> Dcms { get; set; }
        public virtual DbSet<department> departments { get; set; }
        public virtual DbSet<dropdownitem> dropdownitems { get; set; }
        public virtual DbSet<exp_share> exp_share { get; set; }
        public virtual DbSet<exp_step> exp_step { get; set; }
        public virtual DbSet<exp_step_material> exp_step_material { get; set; }
        public virtual DbSet<exp_step_result> exp_step_result { get; set; }
        public virtual DbSet<exp_type> exp_type { get; set; }
        public virtual DbSet<experiment> experiments { get; set; }
        public virtual DbSet<fragment> fragments { get; set; }
        public virtual DbSet<fragment_map> fragment_map { get; set; }
        public virtual DbSet<fragment_methylation> fragment_methylation { get; set; }
        public virtual DbSet<group> groups { get; set; }
        public virtual DbSet<group_people> group_people { get; set; }
        public virtual DbSet<group_shared> group_shared { get; set; }
        public virtual DbSet<ladder> ladders { get; set; }
        public virtual DbSet<ladder_size> ladder_size { get; set; }
        public virtual DbSet<letter_code> letter_code { get; set; }
        public virtual DbSet<license> licenses { get; set; }
        public virtual DbSet<methylation> methylations { get; set; }
        public virtual DbSet<methylation_backup> methylation_backup { get; set; }
        public virtual DbSet<modifying_company> modifying_company { get; set; }
        public virtual DbSet<modifying_enzyme> modifying_enzyme { get; set; }
        public virtual DbSet<oligo> oligoes { get; set; }
        public virtual DbSet<person> people { get; set; }
        public virtual DbSet<people_license> people_license { get; set; }
        public virtual DbSet<plasmid> plasmids { get; set; }
        public virtual DbSet<plasmid_bundle> plasmid_bundle { get; set; }
        public virtual DbSet<plasmid_feature> plasmid_feature { get; set; }
        public virtual DbSet<plasmid_map> plasmid_map { get; set; }
        public virtual DbSet<plasmid_map_backup> plasmid_map_backup { get; set; }
        public virtual DbSet<primer> primers { get; set; }
        public virtual DbSet<probe> probes { get; set; }
        public virtual DbSet<protocol> protocols { get; set; }
        public virtual DbSet<restri_enzyme> restri_enzyme { get; set; }
        public virtual DbSet<restriction_company> restriction_company { get; set; }
        public virtual DbSet<seq_code> seq_code { get; set; }
    }
}
