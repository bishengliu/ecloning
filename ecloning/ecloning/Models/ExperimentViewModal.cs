using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ExperimentViewModal
    {
        //experiment general info
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        public int types { get; set; }
        public string des { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public List<ExpStep> Steps { get; set; }
        //steps
    }

    public class ExpStep
    {
        ////save to exp_step table
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        public int type_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int step_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int people_id { get; set; }
        public int protocol_id { get; set; }
        public string des { get; set; } //save to exp_step table
        public Nullable<System.DateTime> dt { get; set; } //save to exp_step table


        //save to exp_step_material table
        public int forward_primer_id { get; set; }
        public int reverse_primer_id { get; set; }
        public int probe_id { get; set; }
        public string emzyme_id { get; set; }
        public int plasmid_id { get; set; }
        public int frag1_id { get; set; }
        public int frag2_id { get; set; }
    }
}