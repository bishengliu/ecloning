using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ProtocolViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(1, 100, ErrorMessage = "Version number must between 1 and 100, please enter valid integer Number")]
        public int version { get; set; }
        public int versionref { get; set; }
        public string des { get; set; }
    }
}