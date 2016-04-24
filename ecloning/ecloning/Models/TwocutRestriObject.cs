using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ecloning.Models
{
    public class TwocutRestriObject
    {
        //ATCNNNNGAC

        //ATC is the left
        public string leftSeq { get; set; }

        //GAC is the right
        public string rightSeq { get; set; }

        //number of N is 4
        //4
        public int innerLength { get; set; }
    }
}