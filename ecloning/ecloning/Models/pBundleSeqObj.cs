using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class pBundleSeqObj
    {
        public int bId { get; set; }
        public string bName { get; set; }
        public IList<plamidSeqObj> plasmids { get; set; }
    }

    public class plamidSeqObj
    {
        public int pId { get; set; }
        public string pName { get; set; }
        public int seqCount { get; set; }
        public string  sequence { get; set; }
    }
}