
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
            this.methylations = new HashSet<methylation>();
            this.plasmid_map_backup = new HashSet<plasmid_map_backup>();
            this.plasmid_map = new HashSet<plasmid_map>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        public string sequence { get; set; }
        [Required(ErrorMessage = "Required")]
        public string expression_system { get; set; }
        public string expression_subsystem { get; set; }
        public string promotor { get; set; }
        public string polyA { get; set; }
        [Required(ErrorMessage = "Required")]
        public string resistance { get; set; }
        public string reporter { get; set; }
        [Required(ErrorMessage = "Required")]
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
        public string des { get; set; }
        public string insert_species { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<int> seq_length { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<clone_group> clone_group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<methylation> methylations { get; set; }
        public virtual person person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid_map_backup> plasmid_map_backup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid_map> plasmid_map { get; set; }
    }
}
