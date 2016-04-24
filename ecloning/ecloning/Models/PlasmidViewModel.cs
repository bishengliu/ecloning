using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.Models
{
    public class PlasmidViewModel
    {
        public PlasmidViewModel()
        {
            this.clone_group = new HashSet<clone_group>();
            this.plasmid_map = new HashSet<plasmid_map>();
        }
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [RegularExpression("^[ATGCatgc]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C!")]
        public string sequence { get; set; }
        [Required(ErrorMessage = "Required")]
        public int seq_length { get; set; }
        [Required(ErrorMessage = "Required")]
        public string expression_system { get; set; }
        public string expression_subsystem { get; set; }
        public string promotor { get; set; }
        public string polyA { get; set; }
        [Required(ErrorMessage = "Required")]
        public string[] resistance { get; set; }
        public string[] reporter { get; set; }
        public string[] selection { get; set; }
        [Required(ErrorMessage = "Required")]
        public string insert { get; set; }
        [Required(ErrorMessage = "Required")]
        public string[] usage { get; set; }
        public string plasmid_type { get; set; }
        public string ref_plasmid { get; set; }
        public string img_fn { get; set; }
        public Nullable<int> addgene { get; set; }
        public Nullable<System.DateTime> d { get; set; }
        public int people_id { get; set; }
        public string des { get; set; }
        public string insert_species { get; set; }

        public virtual ICollection<clone_group> clone_group { get; set; }
        public virtual person person { get; set; }
        public virtual ICollection<plasmid_map> plasmid_map { get; set; }
    }
}