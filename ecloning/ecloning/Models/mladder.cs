using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.Models
{
    public class mladder
    {
        [Required(ErrorMessage = "Required")]
        public string ladder_type { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        public int company_id { get; set; }
        public string orderref { get; set; }
        public List<ladderSize> ladderSize { get; set; }
    }

    public class ladderSize
    {
        [Required(ErrorMessage = "Required")]
        public int size { get; set; }
        [Required(ErrorMessage = "Required")]
        public double Rf { get; set; }
        [Required(ErrorMessage = "Required")]
        public int mass { get; set; }
    }
}