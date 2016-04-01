namespace ecloning.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class buffer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public buffer()
        {
            this.activity_modifying = new HashSet<activity_modifying>();
            this.activity_restriction = new HashSet<activity_restriction>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        public string des { get; set; }
        [Required(ErrorMessage = "Required")]
        public string composition { get; set; }
        [Required(ErrorMessage = "Required")]
        public int company_id { get; set; }
        public Nullable<bool> show_activity { get; set; }
        public Nullable<bool> show_activity2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<activity_modifying> activity_modifying { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<activity_restriction> activity_restriction { get; set; }
        public virtual company company { get; set; }
    }
}