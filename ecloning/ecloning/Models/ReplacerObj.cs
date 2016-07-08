using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class ReplacerObj
    {
        public int pId { get; set; }
        public IList<int> selection { get; set; }
        public string seq1 { get; set; }
        public string seq2 { get; set; }
    }
}