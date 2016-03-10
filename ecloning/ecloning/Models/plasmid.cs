
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
