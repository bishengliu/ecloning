
namespace ecloning.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public person()
        {
            this.clone_group = new HashSet<clone_group>();
            this.group_people = new HashSet<group_people>();
            this.oligoes = new HashSet<oligo>();
            this.people_license = new HashSet<people_license>();
            this.plasmids = new HashSet<plasmid>();
            this.primers = new HashSet<primer>();
            this.probes = new HashSet<probe>();
            this.projects = new HashSet<project>();
            this.protocols = new HashSet<protocol>();
        }
    
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string first_name { get; set; }
        public string mid_name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string last_name { get; set; }
        public string email { get; set; }
        public string func { get; set; }
        public Nullable<bool> active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<clone_group> clone_group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<group_people> group_people { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<oligo> oligoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<people_license> people_license { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid> plasmids { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<primer> primers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<probe> probes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<project> projects { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<protocol> protocols { get; set; }
    }
}


namespace ecloning.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class plasmid
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public plasmid()
        {
            this.clone_group = new HashSet<clone_group>();
            this.plasmid_map = new HashSet<plasmid_map>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        public string sequence { get; set; }
        [Required(ErrorMessage = "Required")]
        public int seq_length { get; set; }
        [Required(ErrorMessage = "Required")]
        public string expression_system { get; set; }
        public string expression_subsystem { get; set; }
        public string promotor { get; set; }
        public string polyA { get; set; }
        [Required(ErrorMessage = "Required")]
        public string resistance { get; set; }
        public string reporter { get; set; }
        public string selection { get; set; }
        [Required(ErrorMessage = "Required")]
        public string insert { get; set; }
        [Required(ErrorMessage = "Required")]
        public string usage { get; set; }
        public string plasmid_type { get; set; }
        public string ref_plasmid { get; set; }
        public string img_fn { get; set; }
        public Nullable<int> addgene { get; set; }
        public Nullable<System.DateTime> d { get; set; }
        public int people_id { get; set; }
        public Nullable<bool> submitted_to_group { get; set; }
        public Nullable<bool> shared_with_group { get; set; }
        public string shared_with_people { get; set; }
        public string des { get; set; }
        public string insert_species { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<clone_group> clone_group { get; set; }
        public virtual person person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid_map> plasmid_map { get; set; }
    }
}
