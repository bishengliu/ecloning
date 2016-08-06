using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.Models
{
    public class CollaborationExp
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int exp_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public int[] person { get; set; }
    }
}