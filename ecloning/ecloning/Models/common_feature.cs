
namespace ecloning.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class common_feature
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public common_feature()
        {
            this.plasmid_map_backup = new HashSet<plasmid_map_backup>();
            this.plasmid_map = new HashSet<plasmid_map>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int feature_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string label { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[ATGCatgc]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C!")]
        public string sequence { get; set; }
        public string des { get; set; }
        [Required(ErrorMessage = "Required")]
        public int group_id { get; set; }
        public Nullable<int> people_id { get; set; }

        public virtual plasmid_feature plasmid_feature { get; set; }
        public virtual group group { get; set; }
        public virtual person person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid_map_backup> plasmid_map_backup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<plasmid_map> plasmid_map { get; set; }
    }
}
