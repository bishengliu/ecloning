using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class FragmentViewModel
    {
        public int id { get; set; }
        public int plasmid_id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string fName { get; set; }
        public List<string> enzymes { get; set; }
        public int f_start { get; set; }        
        public int f_end { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[ATGCatgcRKBYSDWHNMVrkbysdwhnmv]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C, R, K, B, Y, S, D, W, H, N, M, V!")]
        public string fSeq { get; set; }
        [Required(ErrorMessage = "Required")]
        [Range(-10, 10, ErrorMessage = "Please enter valid integer Number")]
        public int left_overhang { get; set; }
        public List<int> overhangs { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[ATGCatgcRKBYSDWHNMVrkbysdwhnmv]*$", ErrorMessage = "Sequence can only contains letters: A, T, G, C, R, K, B, Y, S, D, W, H, N, M, V!")]
        public string cSeq { get; set; }

        //feature array
        public IList<fragmentFeatures> featureArray { get; set; }
    }
}