using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ecloning.Models
{
    public class dPrimer
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string sequence { get; set; }
        public string company { get; set; }
        public string orderref { get; set; }
        public string location { get; set; }
        public string usage { get; set; }
        public string purity { get; set; }
        public string modification { get; set; }
        public string des { get; set; }
    }
}