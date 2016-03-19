using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.Models
{
    public class FeatureViewModel
    {
        //[Required(ErrorMessage = "Required")]
        //public int feature_id { get; set; }
        //[Required(ErrorMessage = "Required")]
        public int common_id { get; set; }
        //[Required(ErrorMessage = "Required")]
        //public int plasmid_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int start { get; set; }
        [Required(ErrorMessage = "Required")]
        public int end { get; set; }        
        public int? cut { get; set; }
        [Required(ErrorMessage = "Required")]
        public int clockwise { get; set; }
        [Required(ErrorMessage = "Required")]
        public int show_feature { get; set; }

    }
}