using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Areas.Admin.Models
{
    using ecloning.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Restriction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Restriction()
        {
            this.activity_restriction = new HashSet<activity_restriction>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[ATGCatgcRKBYSDWHNMVrkbysdwhnmv]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C, R, K, B, Y, S, D, W, H, N, M, V!")]
        public string forward_seq { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(-20, 20, ErrorMessage = "Please enter valid integer Number")]
        public int forward_cut { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(-20, 20, ErrorMessage = "Please enter valid integer Number")]
        public int reverse_cut { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<bool> staractitivity { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<int> inactivation { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<bool> dam { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<bool> dcm { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<bool> cpg { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<activity_restriction> activity_restriction { get; set; }
    }
}