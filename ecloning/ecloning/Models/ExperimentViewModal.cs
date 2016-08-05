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
        public string des { get; set; }
    }

    public class ExpStep
    {
        public int types { get; set; }
        ////save to exp_step table
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        public int type_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int exp_id { get; set; }
        public int? protocol_id { get; set; }
        public string des { get; set; } //save to exp_step table
        public Nullable<System.DateTime> dt { get; set; } //save to exp_step table

        //save to exp_step_material table
        public int? forward_primer_id { get; set; }
        public int? reverse_primer_id { get; set; }
        public int? probe_id { get; set; }
        public string emzyme_id { get; set; }
        public int? plasmid_id { get; set; }
        public int? frag1_id { get; set; }
        public int? frag2_id { get; set; }
        public int? ligation_method { get; set; } // 1 is direct ligation, 2 is first blunting then ligation, 3 both direct and indirect ligation
        public int? ligation_direction { get; set; } //direct ligation (first pos):10 both direction, 20 postive direction, 30 negative direction; --indirect ligation (2nd pos): 01 both direction, 02 postive direction, 03 negative direction \\\\
        public string plasmidName { get; set; }
    }
    public class ExpType
    {
        public int id { get; set; }
        public string Name { get; set; }
    }
}