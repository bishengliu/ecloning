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
        public string step_owner { get; set; }
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

        public string ligation_method { get; set; } // "X" is not selected, "D" is direct ligation; "I" is first blunting then ligation; "B" both direct and indirect ligation
        public string ligation_direction { get; set; } //"XX" is not selected, direct ligation (first pos): "BX" both direction, "CX" postive direction, "AX" negative direction; --indirect ligation (2nd pos): "XB" both direction, "XC" postive direction, "XA" negative direction \\\\
        public string map1_seq { get; set; }
        public string map2_seq { get; set; }
        public string map3_seq { get; set; }
        public string map4_seq { get; set; }
        public string plasmidName { get; set; }
        public string nplasmid_id { get; set; } //generated plasmids id

        //saved result
        public bool hasResult { get; set; }
        public List<ExpResult> results { get; set;}
    }

    public class ViewExp
    {
        //experiment general info
        public int id { get; set; }
        public string name { get; set; }
        public string des { get; set; }
        public string owner { get; set; }
        public Nullable<System.DateTime> dt { get; set; }
        public List<ExpStep> steps { get; set; }        
    }

    public class ExpResult
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int exp_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int exp_step_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int type_id { get; set; }
        public int result_id { get; set; }
        public string result_upload { get; set; }
        public string result_des { get; set; }
        [Required(ErrorMessage = "Required")]
        public Nullable<System.DateTime> result_dt { get; set; }
    }

    public class ExpType
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    public class ExpTypes
    {
        public string getName(int id)
        {
            var name = "";
            switch (id)
            {
                case 1:
                    name = "Restriction Enzyme Digestion";
                    break;
                case 2:
                    name = "Plasmid Transformation";
                    break;
                case 3:
                    name = "Plasmid Miniprep";
                    break;
                case 4:
                    name = "Fragment Gel Extraction";
                    break;
                case 5:
                    name = "PCR";
                    break;
                case 6:
                    name = "Ligation";
                    break;
                case 7:
                    name = "Pick Colonies";
                    break;
                case 8:
                    name = "Plasmid Maxiprep";
                    break;
                default:
                    name = "";
                    break;

            }

            return name;

        }
    }
}