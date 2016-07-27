using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ProbeViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[ATGCatgcRKBYSDWHNMVrkbysdwhnmv]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C, R, K, B, Y, S, D, W, H, N, M, V!")]
        public string sequence { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(0, 9999, ErrorMessage = "Please enter valid integer Number")]
        public int forward_primer { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(0, 9999, ErrorMessage = "Please enter valid integer Number")]
        public int reverse_primer { get; set; }
        public string usage { get; set; }
        public string location { get; set; }
        public string des { get; set; }
    }
}